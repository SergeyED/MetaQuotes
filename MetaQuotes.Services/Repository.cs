using System;
using System.Diagnostics;
using MetaQuotes.Models;
using MetaQuotes.Models.Version2;

namespace MetaQuotes.Services
{
    /// <summary>
    /// Метод для чтения БД в память и разбивание на массивы
    /// </summary>
    public class Repository : IRepository
    {
        private BinaryGeoBase _db;
        private readonly IFileReadService _fileReadService;
        private readonly IConverterService _converterService;

        public Repository(IFileReadService fileReadService, IConverterService converterService)
        {
            _fileReadService = fileReadService;
            _converterService = converterService;
        }

        public Repository()
        {
        }

        public BinaryGeoBase Db => _db;

        public void Load(string filePath)
        {
            var timer = Stopwatch.StartNew();
            timer.Start();

            byte[] binary = _fileReadService.ReadAllBytes(filePath);

            var header = _converterService.ConvertToHeader(binary, 0);

            if ((int)header.OffsetRanges > binary.Length)
                throw new IndexOutOfRangeException("Длинна бинарника меньше offset для заголовка");
            var ipRanges = new uint[header.Records, 3];
            Buffer.BlockCopy(binary, (int)header.OffsetRanges, ipRanges, 0, header.Records * Constants.IpRangeSize);

            if ((int)header.OffsetLocations > binary.Length)
                throw new IndexOutOfRangeException("Длинна бинарника меньше offset для городов");
            var cities = new byte[header.Records, Constants.CitySize];
            Buffer.BlockCopy(binary, (int)header.OffsetLocations, cities, 0, header.Records * Constants.CitySize);

            if ((int)header.OffsetCities > binary.Length)
                throw new IndexOutOfRangeException("Длинна бинарника меньше offset для городов");
            var locations = new int[header.Records];
            Buffer.BlockCopy(binary, (int)header.OffsetCities, locations, 0, header.Records * Constants.IndexSize);

            _db = new BinaryGeoBase(header, ipRanges, cities, locations);

            timer.Stop();

            _db.LoadStatistic = new BaseLoadStatistic
            {
                LoadDbFromDiskTime = timer.Elapsed
            };
        }
    }
}