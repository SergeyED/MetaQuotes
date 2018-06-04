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
        [ResponseCache(VaryByQueryKeys = new[] { "ip" }, Duration = 86400)] //Кеш на 1 день
        public JsonResult GetByIp(string ip)
        {
            var response = _searchService.SearchByIpAddress(ip);
            return Json(response);
        }

        [HttpGet]
        [Route("city/locations")]
        [ResponseCache(VaryByQueryKeys = new[] { "city" }, Duration = 86400)] //кеш на 1 день
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
                var response = db.LoadStatistic;
                return Json(response);
            }

            return Json(null);
        }


        [HttpGet]
        [Route("api/checkip")]
        public JsonResult CheckIp()
        {
            GeoBase db;
            if (_memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out db))
            {
                var rangeIPStrings = new StringBuilder();
                foreach (var item in db.Ranges)
                {
                    var text = string.Format("{0}: {1} - {2}", item.LocationIndex, IpAddressHelpers.IpUintToString(item.IpFrom), IpAddressHelpers.IpUintToString(item.IpTo));
                    rangeIPStrings.AppendLine(text);
                }

                return Json(rangeIPStrings.ToString());
            }

            return Json("Sorry");
        }
    }
}
