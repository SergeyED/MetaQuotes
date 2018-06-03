using System;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Linq;
using System.Collections.Generic;
namespace MetaQuotes.Services
{
    public class SearchService : ISearchService
    {
        readonly IMemoryCache memoryCache;

        public SearchService(IMemoryCache cache)
        {
            memoryCache = cache;
        }

        public List<City> SearchByCityName(string city)
        {
            if (memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out GeoBase db))
            {
                var cities = db.Cities.Where(x => x.CityName == city).ToList();
                return cities;
            }

            return null;
        }

        public List<City> SearchByIpAddress(string ip)
        {
            var intAddress = IpAddressHelpers.IpToUint(ip);

            if (memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out GeoBase db))
            {
                var ipAddresses = db.Ranges.Where(x => intAddress >= x.IpFrom && intAddress <= x.IpTo);
                if (ipAddresses.Any())
                {
                    var result = new List<City>();
                    foreach (var ipAddress in ipAddresses)
                    {
                        if (db.Cities.Length > ipAddress.LocationIndex)
                        {
                            var city = db.Cities[ipAddress.LocationIndex];
                            result.Add(city);
                        }
                    }
                    return result;
                }
            }

            return null;
        }
    }
}
