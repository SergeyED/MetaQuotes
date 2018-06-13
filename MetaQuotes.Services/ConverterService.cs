using System;
using System.Text;
using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public class ConverterService : IConverterService
    {
        public City ConvertToCity(byte[] binary, int offset)
        {
            if (binary.Length <= offset)
                throw new ArgumentOutOfRangeException("Переданный offset больше длинны массива байт");

            if (binary.Length < (offset + Constants.CitySize))
                throw new ArgumentOutOfRangeException("Offset некорректный, т.к. размер объекта city больше");
            
            var country = Encoding.Default.GetString(binary, offset, 8).TrimEnd('\0');
            offset += 8;
            var region = Encoding.Default.GetString(binary, offset, 12).TrimEnd('\0');
            offset += 12;
            var postal = Encoding.Default.GetString(binary, offset, 8).TrimEnd('\0');
            offset += 12;
            var cityName = Encoding.Default.GetString(binary, offset, 24).TrimEnd('\0');
            offset += 24;
            var organization = Encoding.Default.GetString(binary, offset, 32).TrimEnd('\0');
            offset += 32;
            var latitude = BitConverter.ToSingle(binary, offset);
            offset += 4;
            var longitude = BitConverter.ToSingle(binary, offset);

            var city = new City(country, region, postal, cityName, organization, latitude, longitude);

            return city;
        }

        public Header ConvertToHeader(byte[] binary, int offset)
        {
            if (binary.Length <= offset)
                throw new ArgumentOutOfRangeException("Переданный offset больше длинны массива байт");

            if (binary.Length < (offset + Constants.CitySize))
                throw new ArgumentOutOfRangeException("Offset некорректный, т.к. размер объекта header больше");

            var version = BitConverter.ToInt32(binary, offset);
            offset += 4;
            var name = Encoding.Default.GetString(binary, offset, 32).TrimEnd('\0');
            offset += 32;
            var timestamp = BitConverter.ToUInt64(binary, offset);
            offset += 8;
            var records = BitConverter.ToInt32(binary, offset);
            offset += 4;
            var offsetRanges = BitConverter.ToUInt32(binary, offset);
            offset += 4;
            var offsetCities = BitConverter.ToUInt32(binary, offset);
            offset += 4;
            var offsetLocations = BitConverter.ToUInt32(binary, offset);

            var header = new Header(version, name, timestamp, records, offsetRanges, offsetCities, offsetLocations);

            return header;
        }
    }

}