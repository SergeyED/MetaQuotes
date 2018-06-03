using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using MetaQuotes.Models;
using MetaQuotes.Services.Exceptions;

namespace MetaQuotes.Services
{
    public class BinaryLoader : IBinaryLoader
    {
        public GeoBase LoadDb(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new Exception("Не указан путь к базе");

            if (!File.Exists(filePath))
                throw new Exception("Файла с базой не существует по указанному пути");

            var db = ReadBinaryFile(filePath);

            return db;
        }
        
        private GeoBase ReadBinaryFile(string filePath)
        {
            var db = new GeoBase();

            var timer = Stopwatch.StartNew();
            timer.Start();

            var file = File.ReadAllBytes(filePath);

            timer.Stop();
            db.LoadStatistic.LoadDbFromDiskTime = timer.Elapsed;

            timer.Restart();

            try
            {
                db.Header = ConvertToHeader(file, 0);
            }
            catch (Exception exception)
            {
                throw new ConvertBinaryToClassException("Ошибка при конвертировании Header", exception);
            }

            try
            {
                db.Ranges = ConvertToIpRanges(file, (int) db.Header.OffsetRanges, db.Header.Records);
            }
            catch (Exception exception)
            {
                throw new ConvertBinaryToClassException("Ошибка при конвертировании IpRange", exception);
            }

            try
            {
                db.Cities = ConvertToCities(file, (int) db.Header.OffsetLocations, db.Header.Records);
            }
            catch (Exception exception)
            {
                throw new ConvertBinaryToClassException("Ошибка при конвертировании City", exception);
            }

            try
            {
                db.Locations = ConvertToLocations(file, (int) db.Header.OffsetCities, db.Header.Records);
            }
            catch (Exception exception)
            {
                throw new ConvertBinaryToClassException("Ошибка при конвертировании Location", exception);
            }

            timer.Stop();
            db.LoadStatistic.ConvertBytesToObjectsTime = timer.Elapsed;

            Array.Clear(file, 0, file.Length);

            return db;
        }

        private Header ConvertToHeader(byte[] file, int offset)
        {
            var header = new Header();
            header.Version = BitConverter.ToInt32(file, offset);
            offset += 4;
            header.Name = new string(Encoding.Default.GetChars(file, offset, 32)).TrimEnd('\0');
            offset += 32;
            header.Timestamp = BitConverter.ToUInt64(file, offset);
            offset += 8;
            header.Records = BitConverter.ToInt32(file, offset);
            offset += 4;
            header.OffsetRanges = BitConverter.ToUInt32(file, offset);
            offset += 4;
            header.OffsetCities = BitConverter.ToUInt32(file, offset);
            offset += 4;
            header.OffsetLocations = BitConverter.ToUInt32(file, offset);

            return header;
        }

        private IpRange ConvertToIpRange(byte[] file, int offset)
        {
            var IpRange = new IpRange();
            IpRange.IpFrom = BitConverter.ToUInt32(file, offset);
            offset += 4;
            IpRange.IpTo = BitConverter.ToUInt32(file, offset);
            offset += 4;
            IpRange.LocationIndex = BitConverter.ToUInt32(file, offset);

            return IpRange;
        }

        private City ConvertToCity(byte[] file, int offset)
        {
            var city = new City();
            city.Country = Encoding.Default.GetString(file, offset, 8).TrimEnd('\0');
            offset += 8;
            city.Region = Encoding.Default.GetString(file, offset, 12).TrimEnd('\0');
            offset += 12;
            city.Postal = Encoding.Default.GetString(file, offset, 8).TrimEnd('\0');
            offset += 12;
            city.CityName = Encoding.Default.GetString(file, offset, 24).TrimEnd('\0');
            offset += 24;
            city.Organization = Encoding.Default.GetString(file, offset, 32).TrimEnd('\0');
            offset += 32;
            city.Latitude = BitConverter.ToSingle(file, offset);
            offset += 4;
            city.Longitude = BitConverter.ToSingle(file, offset);

            return city;
        }

        private Location ConvertToLocation(byte[] file, int offset)
        {
            var location = new Location();
            location.Index = BitConverter.ToUInt32(file, offset);

            return location;
        }

        private IpRange[] ConvertToIpRanges(byte[] file, int offset, int count)
        {
            var ipRanges = new IpRange[count];
            for (int i = 0; i < count; i++)
            {
                var ipRange = ConvertToIpRange(file, offset);
                ipRanges[i] = ipRange;
                offset += 12;
            }

            return ipRanges;
        }

        private City[] ConvertToCities(byte[] file, int offset, int count)
        {
            var cities = new City[count];
            for (int i = 0; i < count; i++)
            {
                var city = ConvertToCity(file, offset);
                cities[i] = city;
                offset += 96;
            }

            return cities;
        }

        private Location[] ConvertToLocations(byte[] file, int offset, int count)
        {
            var locations = new Location[count];
            for (int i = 0; i < count; i++)
            {
                var location = ConvertToLocation(file, offset);
                locations[i] = location;
                offset += 4;
            }

            return locations;
        }
    }

}

