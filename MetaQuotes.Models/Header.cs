using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    public class Header
    {
        /// <summary>
        /// версия база данных
        /// </summary>
        public int Version { get; set; }
       
        /// <summary>
        /// название/префикс для базы данных
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// время создания базы данных
        /// </summary>
        public ulong Timestamp { get; set; }
        
        /// <summary>
        /// общее количество записей
        /// </summary>
        public int Records { get; set; }
        
        /// <summary>
        /// смещение относительно начала файла до начала списка записей с геоинформацией.
        /// </summary>
        public uint OffsetRanges { get; set; }
        
        /// <summary>
        /// смещение относительно начала файла до начала индекса с сортировкой по названию городов
        /// </summary>
        public uint OffsetCities { get; set; }
        
        /// <summary>
        /// смещение относительно начала файла до начала списка записей о местоположении
        /// </summary>
        public uint OffsetLocations { get; set; }
    }
}
