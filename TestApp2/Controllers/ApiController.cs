using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using MetaQuotes.Services;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;

namespace TestApp2.Controllers
{
    public class ApiController : Controller
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ISearchService _searchService;

        public ApiController(IMemoryCache cache, ISearchService searchService)
        {
            _memoryCache = cache;
            _searchService = searchService;
        }

        [HttpGet]
        [Route("ip/location")]
        public JsonResult GetByIp(string ip)
        {
            //var response = _searchService.SearchByIpAddress(ip);
            var response = _searchService.BinarySearchByIpAddress(ip);
            return Json(response);
        }

        [HttpGet]
        [Route("city/locations")]
        public JsonResult GetByCity(string city)
        {
            var response = _searchService.SearchByCityName(city);
            return Json(response);
        }

        [HttpGet]
        [Route("api/stat")]
        public JsonResult GetStat()
        {
            GeoBase db;
            if (_memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out db))
            {
                var response = new
                {
                    LoadDbFromDiskTime = db.LoadStatistic.LoadDbFromDiskTime.Milliseconds,
                    ConvertBytesToObjectsTime = db.LoadStatistic.ConvertBytesToObjectsTime.Milliseconds
                };
                return Json(response);
            }

            return Json(null);
        }
    }
}
