using System;
using MetaQuotes.Models;
using System.IO;

namespace MetaQuotes.Services.Tests
{
    /// <summary>
    /// Данные для тестов
    /// </summary>
    public static class GeoBaseTestData
    {
        public static string DbName = "GeoBase"; 
        public static byte[] GetBinaryGeoBase()
        {
            var ipRanges = new IpRange[] {
                new IpRange(8, 1000, 0),
                new IpRange(1001, 9999, 1)
            };

            var cities = new City[]{
                new City("con_1", "reg_1", "pos_1", "cit_1", "org_1", 0.1f, 0.1f),
                new City("con_2", "reg_2", "pos_2", "cit_2", "org_2", 0.2f, 0.2f)
            };

            var locations = new Location[] {
                new Location(0),
                new Location(1)
            };

            var offsetCities = Constants.HeaderSize + (uint)(Constants.IpRangeSize * ipRanges.Length);
            var offsetLocations = offsetCities + (uint)(Constants.CitySize * cities.Length);
            var header = new Header(1, DbName, (ulong)DateTime.Now.AddDays(-1).Ticks, 2, Constants.HeaderSize, offsetCities, offsetLocations);

            var binary = SerializeToByte(header, ipRanges, cities, locations);
            return binary;
        }

        private static char[] GetCharsToFixArraySize(string text, int size){
            var chars = new char[size];
            var textChars = text.ToCharArray();
            for (int i = 0; i < textChars.Length; i++)
            {
                chars[i] = text[i];
            }

            return chars;
        }

        private static byte[] SerializeToByte(Header header, IpRange[] ipRanges, City[] cities, Location[] locations)
        {
            byte[] bytes = null;

            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(header.Version);
                    writer.Write(GetCharsToFixArraySize(header.Name,32));
                    writer.Write(header.Timestamp);
                    writer.Write(header.Records);
                    writer.Write(header.OffsetRanges);
                    writer.Write(header.OffsetCities);
                    writer.Write(header.OffsetLocations);

                    for (int i = 0; i < ipRanges.Length; i++)
                    {
                        writer.Write(ipRanges[i].IpFrom);
                        writer.Write(ipRanges[i].IpTo);
                        writer.Write(ipRanges[i].LocationIndex);
                    }

                    for (int i = 0; i < cities.Length; i++)
                    {
                        writer.Write(GetCharsToFixArraySize(cities[i].Country, 8));
                        writer.Write(GetCharsToFixArraySize(cities[i].Region, 12));
                        writer.Write(GetCharsToFixArraySize(cities[i].Postal, 12));
                        writer.Write(GetCharsToFixArraySize(cities[i].CityName, 24));
                        writer.Write(GetCharsToFixArraySize(cities[i].Organization, 32));
                        writer.Write(cities[i].Latitude);
                        writer.Write(cities[i].Longitude);
                    }

                    for (int i = 0; i < locations.Length; i++)
                    {
                        writer.Write(locations[i].Index);
                    }
                }

                bytes = stream.ToArray();
            }

            return bytes;
        }

    }
}
