using System;

namespace MetaQuotes.Models
{
    /// <summary>
    /// Статистика загрузки данных
    /// </summary>
    public class BaseLoadStatistic
    {
        /// <summary>
        /// Время загрузки бинарного файла в память
        /// </summary>
        public TimeSpan LoadDbFromDiskTime { get; set; }

        /// <summary>
        /// Время конвертирования бинарных данных в объекты
        /// </summary>
        public TimeSpan ConvertBytesToObjectsTime { get; set; }
    }
}