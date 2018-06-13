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
            var response = _searchService.BinarySearchByCityName(city);
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
    }
}
