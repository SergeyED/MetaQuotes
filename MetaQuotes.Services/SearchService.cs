using System;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using MetaQuotes.Models.Version2;
namespace MetaQuotes.Services
{
    public class SearchService : ISearchService
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IConverterService _converterService;
        private readonly IRepository _repository;

        public SearchService(IMemoryCache memoryCache, IConverterService converterService, IRepository repository)
        {
            _memoryCache = memoryCache;
            _converterService = converterService;
            _repository = repository;
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

                var midRangeFrom = db.IpRanges[midIndex, 0];
                var midRangeTo = db.IpRanges[midIndex, 1];

                if (intAddress >= midRangeFrom && intAddress <= midRangeTo)
                {
                    var index = db.IpRanges[midIndex, 2];
                    return GetCityByIndex(db, index);
                }

                if (intAddress < midRangeFrom){
                    lastIndex = midIndex - 1;
                } else {
                    firstIndex = midIndex + 1;
                }
            }
            return null;
        }

        private City GetCityByIndex(BinaryGeoBase db, uint index)
        {
            byte[] cityObject = new byte[96];
            for (int i = 0; i < 96; i++)
            {
                cityObject[i] = db.Cities[index, i];
            }
            var city = _converterService.ConvertToCity(cityObject, 0);
            return city;
        }

        /// <summary>
        /// Бинарный поиск города по байтам
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public List<City> BinarySearchByCityName(string cityName)
        {
            var cities = new List<City>();
            //if (!_memoryCache.TryGetValue(CacheConstants.BinaryGeoBaseKey, out BinaryGeoBase db)) return null;

            byte[] searchBytes = new byte[24];
            searchBytes = Encoding.ASCII.GetBytes(cityName.PadRight(24, '\0')); //Добиваем искомый город нолями справа для сравнения

            var firstIndex = 0;
            var lastIndex = _repository.Db.Locations.Length;
            SearchByCurrentIndexes(cities, _repository.Db, searchBytes, firstIndex, lastIndex);
            return cities;
        }

        private void SearchByCurrentIndexes(List<City> cities, BinaryGeoBase db, byte[] searchBytes, int firstIndex, int lastIndex)
        {
            while (firstIndex < lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midLocation = db.Locations[midIndex];
                byte[] binaryCity = GetCityForCompare(db, midLocation);

                var result = searchBytes.SequenceEqual(binaryCity);
                if (result != 0)
                {
                    if (result > 0)
                    {
                        lastIndex = midIndex - 1;
                        break;
                    }
                    else
                    {
                        firstIndex = midIndex + 1;
                        break;
                    }
                    
                    //for (int i = 0; i < searchBytes.Length; i++)
                    //{
                    //    if (searchBytes[i] == binaryCity[i])
                    //    {
                    //        //если части слова повторяются, идем до отличия чтобы понять куда будет смещение
                    //        continue;
                    //    }

                    //    if (searchBytes[i] > binaryCity[i])
                    //    {
                    //        firstIndex = midIndex + 1;
                    //        break;
                    //    }

                    //    lastIndex = midIndex - 1; ;
                    //    break;
                    //}
                }

                City city = GetConvertedCity(db, midLocation);
                cities.Add(city);

                //Продолжить поиск в обе стороны, т.к. город может повторяться, а так как они отсортированы, 
                //то идем до первого отличия
                SearchCityToRight(cities, db, searchBytes, lastIndex, midIndex);
                SearchCityToLeft(cities, db, searchBytes, firstIndex, midIndex);

                return;
            }
        }

        private void SearchCityToRight(List<City> cities, BinaryGeoBase db, byte[] searchBytes, int lastIndex, int rightOffset)
        {
            while (lastIndex >= rightOffset)
            {
                rightOffset++;
                var midLocation = db.Locations[rightOffset];
                var binaryCity = GetCityForCompare(db, midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(GetConvertedCity(db, midLocation));
                }
                else
                {
                    break;
                    //Если попали сюда, значит у нас больше нет городов с таким названием в этом напрпавлении
                }
            }
        }

        private void SearchCityToLeft(List<City> cities, BinaryGeoBase db, byte[] searchBytes, int firstIndex, int leftOffset)
        {
            while (firstIndex <= leftOffset)
            {
                leftOffset--;
                var midLocation = db.Locations[leftOffset];
                var binaryCity = GetCityForCompare(db, midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(GetConvertedCity(db, midLocation));
                }
                else
                {
                    break;
                    //Если попали сюда, значит у нас больше нет городов с таким названием в этом напрпавлении
                }
            }
        }


        private City GetConvertedCity(BinaryGeoBase db, int midLocation)
        {
            byte[] cityBytes = new byte[96];
            Buffer.BlockCopy(db.Cities, midLocation, cityBytes, 0, 96);
            var city = _converterService.ConvertToCity(cityBytes, 0);
            return city;
        }

        private static byte[] GetCityForCompare(BinaryGeoBase db, int midLocation)
        {
            var binaryCity = new byte[24];
            Buffer.BlockCopy(db.Cities, midLocation + 32, binaryCity, 0, 24);
            return binaryCity;
        }
    }
}
