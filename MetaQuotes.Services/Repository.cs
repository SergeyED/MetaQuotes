using System;
using System.Diagnostics;
using System.IO;
using MetaQuotes.Models.Version2;
using MetaQuotes.Services.Common;

namespace MetaQuotes.Services
{
    /// <summary>
    /// Экспериментальый метод для чтения БД в память и разбивание на массивы, по которым планировалось, что будет происходить поиск.
    /// Но скорость загрузки и разбивки по 3-м коллекциям составляет более 70 мс.
    /// </summary>
    public class Repository : IRepository
    {
        private BinaryGeoBase _db;

        public BinaryGeoBase Db => _db;

        public void Load(string filePath)
        {
            var timer = Stopwatch.StartNew();
            timer.Start();

            if (string.IsNullOrWhiteSpace(filePath))
                throw new FileNotFoundException("Не указано имя файла");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не существует по указанному пути: {filePath}");

            var file = File.ReadAllBytes(filePath);
            var header = file.ReadStruct<HeaderBuffer>(0);
            if (header.Equals(default(HeaderBuffer)))
            {
                throw new Exception("Не удалось сконвертировать структуру Header");
            }

            var ipRanges = new uint[header.Records, 3];
            Buffer.BlockCopy(file, (int)header.OffsetRanges, ipRanges, 0, header.Records * 12);

            var cities = new byte[header.Records, 96];
            Buffer.BlockCopy(file, (int)header.OffsetLocations, cities, 0, header.Records * 96);

            var locations = new int[header.Records];
            Buffer.BlockCopy(file, (int)header.OffsetCities, locations, 0, header.Records * 4);

            _db = new BinaryGeoBase(header, ipRanges, cities, locations);

            timer.Stop();

            _db.LoadStatistic = new Models.BaseLoadStatistic();
            _db.LoadStatistic.LoadDbFromDiskTime = timer.Elapsed;
        }
    }
}