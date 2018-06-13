using System;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;
using System.Text;
using System.Linq;
namespace MetaQuotes.Services
{
    public class SearchService : ISearchService
    {
        private readonly IConverterService _converterService;
        private readonly IRepository _repository;

        public SearchService(IConverterService converterService, IRepository repository)
        {
            _converterService = converterService;
            _repository = repository;
        }

        public City[] BinarySearchByIpAddress(string ip)
        {
            var intAddress = IpAddressHelpers.IpToUint(ip);

            if (_repository.Db == null)
                throw new Exception("База данных не загружена");

            var firstIndex = 0;
            var lastIndex = _repository.Db.Header.Records;

            while (firstIndex <= lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midRangeFrom = _repository.Db.IpRanges[midIndex, 0];
                var midRangeTo = _repository.Db.IpRanges[midIndex, 1];

                if (intAddress >= midRangeFrom && intAddress <= midRangeTo)
                {
                    var index = _repository.Db.IpRanges[midIndex, 2];
                    return new City[] { _converterService.ConvertToCity(_repository.Db.Cities, (int)index * Constants.CitySize) };
                }

                if (intAddress < midRangeFrom)
                {
                    lastIndex = midIndex - 1;
                }
                else
                {
                    firstIndex = midIndex + 1;
                }
            }
            return new City[] { };
        }

        /// <summary>
        /// Бинарный поиск города по байтам
        /// </summary>
        /// <param name="cityName"></param>
        /// <returns></returns>
        public City[] BinarySearchByCityName(string cityName)
        {
            var searchBytes = Encoding.Default.GetBytes(cityName.PadRight(24, '\0'));

            var firstIndex = 0;
            var lastIndex = _repository.Db.Locations.Length;

            var cities = SearchCityByIndexes(searchBytes, firstIndex, lastIndex);

            var result = cities.ToArray();

            return result;
        }


        private List<City> SearchCityByIndexes(byte[] searchBytes, int firstIndex, int lastIndex)
        {
            var cities = new List<City>();
            while (firstIndex <= lastIndex)
            {
                int midIndex = firstIndex + (lastIndex - firstIndex) / 2;

                var midLocation = _repository.Db.Locations[midIndex];
                byte[] binaryCity = GetBinaryCityForCompare(midLocation);

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

                var city = _converterService.ConvertToCity(_repository.Db.Cities, midLocation);
                cities.Add(city);

                //Продолжить поиск в обе стороны, т.к. город может повторяться, а так как они отсортированы, 
                //то идем до первого отличия
                SearchCityToRight(cities, searchBytes, lastIndex, midIndex);
                SearchCityToLeft(cities, searchBytes, firstIndex, midIndex);

                return cities;
            }

            return cities;
        }

        private void SearchCityToRight(List<City> cities, byte[] searchBytes, int lastIndex, int rightOffset)
        {
            while (lastIndex >= rightOffset)
            {
                rightOffset++;
                var midLocation = _repository.Db.Locations[rightOffset];
                var binaryCity = GetBinaryCityForCompare(midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(_converterService.ConvertToCity(_repository.Db.Cities, midLocation));
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
            if (leftOffset == 0) return;

            while (firstIndex <= leftOffset)
            {
                leftOffset--;
                var midLocation = _repository.Db.Locations[leftOffset];
                var binaryCity = GetBinaryCityForCompare(midLocation);
                if (searchBytes.SequenceEqual(binaryCity) == 0)
                {
                    cities.Add(_converterService.ConvertToCity(_repository.Db.Cities, midLocation));
                }
                else
                {
                    break;
                    //Если попали сюда, значит у нас больше нет городов с таким названием в этом напрпавлении
                }
            }
        }

        private byte[] GetBinaryCityForCompare(int midLocation)
        {
            var binaryCity = _repository.Db.Cities.Skip(midLocation + 32).Take(24).ToArray();

            return binaryCity;
        }
    }
}
