using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public interface IBinaryLoader
    {
        /// <summary>
        /// Загрузка бинарной базы из файла
        /// </summary>
        /// <param name="filePath">Путь к базе данных</param>
        /// <returns></returns>
        GeoBase LoadDb(string filePath);
    }
}
