using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Collections.Generic;
using MetaQuotes.Models.Version2;
namespace MetaQuotes.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConverterService _converterService;

        public SearchService(IMemoryCache memoryCache, IConverterService converterService)
        {
            _memoryCache = memoryCache;
            _converterService = converterService;
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

        public City BinarySearchByIpAddress(string ip)
        {
            if (!_memoryCache.TryGetValue(CacheConstants.BinaryGeoBaseKey, out BinaryGeoBase db)) return null;

            var intAddress = IpAddressHelpers.IpToUint(ip);

            var firstIndex = 0;
            var lastIndex = db.Header.Records;

            while (firstIndex < lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midRangeFrom = db.IpRanges[midIndex, 1];
                var midRangeTo = db.IpRanges[midIndex, 2];

                if (intAddress >= midRangeFrom && intAddress <= midRangeFrom)
                {
                    var index = db.Locations[midIndex];
                    return GetCityByIndex(db, index);
                }

                if (intAddress < midRangeFrom){
                    firstIndex = midIndex - 1;
                } else {
                    lastIndex = midIndex + 1;
                }
            }
            return null;
        }

        private City GetCityByIndex(BinaryGeoBase db, int index)
        {
            byte[] cityObject = new byte[96];
            for (int i = 0; i < 96; i++)
            {
                cityObject[i] = db.Cities[index, i];
            }
            var city = _converterService.ConvertToCity(cityObject, 0);
            return city;
        }

        public List<City> BinarySearchByCityName(string city)
        {
            throw new System.NotImplementedException();
        }
    }
}
