using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    [StructLayout(LayoutKind.Sequential, Size = Constants.HeaderSize)]
    public struct Header
    {
        /// <summary>
        /// версия база данных
        /// </summary>
        public int version; 
        /// <summary>
        /// название/префикс для базы данных
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string name;
        /// <summary>
        /// время создания базы данных
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// общее количество записей
        /// </summary>
        public int records;
        /// <summary>
        /// смещение относительно начала файла до начала списка записей с геоинформацией.
        /// </summary>
        public uint offset_ranges;
        /// <summary>
        /// смещение относительно начала файла до начала индекса с сортировкой по названию городов
        /// </summary>
        public uint offset_cities;
        /// <summary>
        /// смещение относительно начала файла до начала списка записей о местоположении
        /// </summary>
        public uint offset_locations;

    }
}
