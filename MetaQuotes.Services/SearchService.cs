using System;
using MetaQuotes.Models;
using MetaQuotes.Services.Common;
using Microsoft.Extensions.Caching.Memory;
using TestApp2;
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
                var cities = db.Cities.Where(x => x.city == city).ToList();
                return cities;
            }

            return null;
        }

        public List<City> SearchByIpAddress(string ip)
        {
            var intAddress = IpAddressHelpers.IpToUint(ip);

            if (memoryCache.TryGetValue(CacheConstants.GeoBaseKey, out GeoBase db))
            {
                var ipAddresses = db.Ranges.Where(x => intAddress >= x.ip_from && intAddress <= x.ip_to);
                if (ipAddresses.Any())
                {
                    var result = new List<City>();
                    foreach (var ipAddress in ipAddresses)
                    {
                        if (db.Cities.Length > ipAddress.location_index)
                        {
                            var city = db.Cities[ipAddress.location_index];
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
