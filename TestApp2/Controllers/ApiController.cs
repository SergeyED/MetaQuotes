using MetaQuotes.Models;
using MetaQuotes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace MetaQuotes.WebApp.Controllers
{
    public class ApiController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISearchService _searchService;
        private readonly IRepository _repository;

        public ApiController(IMemoryCache cache, ISearchService searchService, IRepository repository)
        {
            _memoryCache = cache;
            _searchService = searchService;
            _repository = repository;
        }

        [HttpGet]
        [Route("ip/location")]
        public JsonResult GetByIp(string ip)
        {
            var response = _searchService.BinarySearchByIpAddress(ip);
            return Json(response);
        }

        [HttpGet]
        [Route("city/locations")]
        public JsonResult GetByCity(string city)
        {
            if (TryGetSearchResultFromCache(CacheConstants.SearchByCityCacheName(city), out City[] citiesFromCache))
                return Json(citiesFromCache);

            var response = _searchService.BinarySearchByCityName(city);

            //Кеширование лучше вынести в отдельный сервис, либо использовать Cache Response, который появился в Asp .Net Core 2.1 
            SetSearchResultToCache(CacheConstants.SearchByCityCacheName(city), response);
            return Json(response);
        }

        /// <summary>
        /// Получение статистики загрузки базы
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/stat")]
        public JsonResult GetStat()
        {
            var response = new
            {
                LoadDbFromDiskTime = _repository.Db.LoadStatistic.LoadDbFromDiskTime.Milliseconds
            };
            return Json(response);
        }

        /// <summary>
        /// Пытаемся достать результаты поиска из кеша
        /// </summary>
        /// <param name="cacheName">Имя кеша генерируется при помощи статичного класса CacheConstants</param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryGetSearchResultFromCache(string cacheName, out City[] result)
        {
            if (_memoryCache.TryGetValue(cacheName, out result))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Добавить результат поика в вечный кеш
        /// </summary>
        /// <param name="cacheName">Имя кеша генерируется при помощи статичного класса CacheConstants</param>
        /// <param name="result"></param>
        private void SetSearchResultToCache(string cacheName, City[] result)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            _memoryCache.Set(cacheName, result, cacheEntryOptions);
        }
    }
}
