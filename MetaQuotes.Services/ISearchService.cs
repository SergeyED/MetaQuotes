using System.Collections.Generic;
using MetaQuotes.Models;
namespace MetaQuotes.Services
{
    public interface ISearchService
    {
        /// <summary>
        /// Поиск по IP адресу
        /// </summary>
        /// <returns>The by ip address.</returns>
        /// <param name="ip">Ip.</param>
        List<City> SearchByIpAddress(string ip);

        /// <summary>
        /// Поиск по точному назвванию города
        /// </summary>
        /// <returns>The by city name.</returns>
        /// <param name="city">City.</param>
        List<City> SearchByCityName(string city);
    }
}
