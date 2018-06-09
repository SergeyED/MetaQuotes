using System;
using System.Text;
using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public class ConverterService : IConverterService
    {
        public City ConvertToCity(byte[] source, int offset)
        {
            var country = Encoding.UTF8.GetString(source, offset, 8).TrimEnd('\0');
            offset += 8;
            var region = Encoding.UTF8.GetString(source, offset, 12).TrimEnd('\0');
            offset += 12;
            var postal = Encoding.UTF8.GetString(source, offset, 8).TrimEnd('\0');
            offset += 12;
            var cityName = Encoding.UTF8.GetString(source, offset, 24).TrimEnd('\0');
            offset += 24;
            var organization = Encoding.UTF8.GetString(source, offset, 32).TrimEnd('\0');
            offset += 32;
            var latitude = BitConverter.ToSingle(source, offset);
            offset += 4;
            var longitude = BitConverter.ToSingle(source, offset);

            var city = new City(country, region, postal, cityName, organization, latitude, longitude);

            return city;
        }
    }

}