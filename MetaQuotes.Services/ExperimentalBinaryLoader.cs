using System;
using System.Collections.Generic;
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
    public class ExperimentalBinaryLoader: IExperimentalBinaryLoader
    {
        public void ReadBinaryFileToByteArray(string filePath)
        {
            var timer = Stopwatch.StartNew();
            timer.Start();

            var file = File.ReadAllBytes(filePath);
            var header = file.ReadStruct<HeaderBuffer>(0);
            if (header.Equals(default(HeaderBuffer)))
            {
                throw new Exception("Не удалось сконвертировать структуру Header");
            }

            var offset = (int)header.OffsetRanges;
            var ipRanges = ReadBinaryToList(header.Records, file, offset, 12);
           
            offset = (int)header.OffsetLocations;
            var cities = ReadBinaryToList(header.Records, file, offset, 96);
            
            offset = (int)header.OffsetLocations;
            var locations = ReadBinaryToList(header.Records, file, offset, 4);

            timer.Stop();
        }

        private IEnumerable<byte[]> ReadBinaryToList(int records, byte[] file, int offset, int listItemSize)
        {
            List<byte[]> list = new List<byte[]>();
            for (int i = 0; i < records; i++)
            {
                var array = new byte[listItemSize];
                Buffer.BlockCopy(file, offset, array, 0, listItemSize);
                list.Add(array);
                offset += listItemSize;
            }

            return list;
        }
    }
}