using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Collections.Generic;
namespace MetaQuotes.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMemoryCache _memoryCache;

        public SearchService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public List<City> SearchByCityName(string city)
        {
            if (TryGetSearchResultFromCache(CacheConstants.SearchByCityCacheName(city), out var resultFromCache))
            {
                return resultFromCache;
            }

            if (!_memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out GeoBase db)) return null;

            var cities = db.Cities.Where(x => x.CityName == city).ToList();

            SetSearchResultToCache(CacheConstants.SearchByCityCacheName(city), cities);

            return cities;
        }

        public List<City> SearchByIpAddress(string ip)
        {
            if (TryGetSearchResultFromCache(CacheConstants.SearchByIpCacheName(ip), out var resultFromCache))
            {
                return resultFromCache;
            }

            var intAddress = IpAddressHelpers.IpToUint(ip);

            if (!_memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out GeoBase db)) return null;

            var ipAddresses = db.Ranges.Where(x => intAddress >= x.IpFrom && intAddress <= x.IpTo).ToList();
            if (ipAddresses.Any())
            {
                var cities = new List<City>();
                foreach (var ipAddress in ipAddresses)
                {
                    if (db.Cities.Length > ipAddress.LocationIndex)
                    {
                        var city = db.Cities[ipAddress.LocationIndex];
                        cities.Add(city);
                    }
                }

                SetSearchResultToCache(CacheConstants.SearchByIpCacheName(ip), cities);

                return cities;
            }

            return null;
        }

        /// <summary>
        /// Пытаемся достать результаты поиска из кеша
        /// </summary>
        /// <param name="cacheName">Имя кеша генерируется при помощи статичного класса CacheConstants</param>
        /// <param name="result"></param>
        /// <returns></returns>
        private bool TryGetSearchResultFromCache(string cacheName, out List<City> result)
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
        private void SetSearchResultToCache(string cacheName, List<City> result)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions().SetPriority(CacheItemPriority.NeverRemove);
            _memoryCache.Set(cacheName, result, cacheEntryOptions);
        }
    }
}
