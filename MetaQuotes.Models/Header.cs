using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    public class Header
    {
        public Header(int version, string name, ulong timestamp, int records, uint offsetRanges, uint offsetCities, uint offsetLocations)
        {
            Version = version;
            Name = name;
            Timestamp = timestamp;
            Records = records;
            OffsetRanges = offsetRanges;
            OffsetCities = offsetCities;
            OffsetLocations = offsetLocations;
        }

        /// <summary>
        /// версия база данных
        /// </summary>
        public int Version { get;  }
       
        /// <summary>
        /// название/префикс для базы данных
        /// </summary>
        public string Name { get; }
        
        /// <summary>
        /// время создания базы данных
        /// </summary>
        public ulong Timestamp { get; }
        
        /// <summary>
        /// общее количество записей
        /// </summary>
        public int Records { get;  }
        
        /// <summary>
        /// смещение относительно начала файла до начала списка записей с геоинформацией.
        /// </summary>
        public uint OffsetRanges { get; }
        
        /// <summary>
        /// смещение относительно начала файла до начала индекса с сортировкой по названию городов
        /// </summary>
        public uint OffsetCities { get;  }
        
        /// <summary>
        /// смещение относительно начала файла до начала списка записей о местоположении
        /// </summary>
        public uint OffsetLocations { get;  }
    }
}
