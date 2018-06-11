using System;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text;
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


        private City GetCityByIndex(uint index)
        {
            byte[] cityObject = new byte[96];
            for (int i = 0; i < 96; i++)
            {
                cityObject[i] = _repository.Db.Cities[index, i];
            }
            var city = _converterService.ConvertToCity(cityObject, 0);
            return city;
        }

        public City[] BinarySearchByIpAddress(string ip)
        {
            //if (!_memoryCache.TryGetValue(CacheConstants.BinaryGeoBaseKey, out BinaryGeoBase db)) return null;

            var intAddress = IpAddressHelpers.IpToUint(ip);

            var firstIndex = 0;
            var lastIndex = _repository.Db.Header.Records;

            while (firstIndex < lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midRangeFrom = _repository.Db.IpRanges[midIndex, 0];
                var midRangeTo = _repository.Db.IpRanges[midIndex, 1];

                if (intAddress >= midRangeFrom && intAddress <= midRangeTo)
                {
                    var index = _repository.Db.IpRanges[midIndex, 2];
                    return new City[] {GetCityByIndex(index)};
                }

                if (intAddress < midRangeFrom){
                    lastIndex = midIndex - 1;
                } else {
                    firstIndex = midIndex + 1;
                }
            }
            return new City[]{};
        }


        /// <summary>
        /// Бинарный поиск города по байтам
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public City[] BinarySearchByCityName(string cityName)
        {
            var cities = new List<City>();
            //if (!_memoryCache.TryGetValue(CacheConstants.BinaryGeoBaseKey, out BinaryGeoBase db)) return null;

            var searchBytes = Encoding.Default.GetBytes(cityName.PadRight(24, '\0'));

            var firstIndex = 0;
            var lastIndex = _repository.Db.Locations.Length;
            SearchByCurrentIndexes(cities, searchBytes, firstIndex, lastIndex);
            return cities.ToArray();
        }

        private void SearchByCurrentIndexes(List<City> cities, byte[] searchBytes, int firstIndex, int lastIndex)
        {
            while (firstIndex < lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midLocation = _repository.Db.Locations[midIndex];
                byte[] binaryCity = GetCityForCompare(midLocation);

                var result = searchBytes.SequenceEqual(binaryCity);
                if (result != 0)
                {
                    if (result < 0)
                    {
                        lastIndex = midIndex - 1;
                        continue;
                    }
                    else
                    {
                        firstIndex = midIndex + 1;
                        continue;
                    }
                    
                }

                var city = ConvertToCity(midLocation);
                cities.Add(city);

                //Продолжить поиск в обе стороны, т.к. город может повторяться, а так как они отсортированы, 
                //то идем до первого отличия
                SearchCityToRight(cities, searchBytes, lastIndex, midIndex);
                SearchCityToLeft(cities, searchBytes, firstIndex, midIndex);

                return;
            }
        }

        private void SearchCityToRight(List<City> cities, byte[] searchBytes, int lastIndex, int rightOffset)
        {
            while (lastIndex >= rightOffset)
            {
                rightOffset++;
                var midLocation = _repository.Db.Locations[rightOffset];
                var binaryCity = GetCityForCompare(midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(ConvertToCity(midLocation));
                }
                else
                {
                    break;
                    //Если попали сюда, значит у нас больше нет городов с таким названием в этом напрпавлении
                }
            }
        }

        private void SearchCityToLeft(List<City> cities, byte[] searchBytes, int firstIndex, int leftOffset)
        {
            while (firstIndex <= leftOffset)
            {
                leftOffset--;
                var midLocation = _repository.Db.Locations[leftOffset];
                var binaryCity = GetCityForCompare(midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(ConvertToCity(midLocation));
                }
                else
                {
                    break;
                    //Если попали сюда, значит у нас больше нет городов с таким названием в этом напрпавлении
                }
            }
        }


        private City ConvertToCity(int midLocation)
        {
            var cityBytes = new byte[96];
            Buffer.BlockCopy(_repository.Db.Cities, midLocation, cityBytes, 0, 96);
            var city = _converterService.ConvertToCity(cityBytes, 0);
            return city;
        }

        private byte[] GetCityForCompare(int midLocation)
        {
            var binaryCity = new byte[24];
            Buffer.BlockCopy(_repository.Db.Cities, midLocation + 32, binaryCity, 0, 24);

            return binaryCity;
        }
    }
}
