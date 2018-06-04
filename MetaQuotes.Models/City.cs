using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    public class City
    {
        public City(string country, string region, string postal, string cityName, string organization, float latitude, float longitude)
        {
            Country = country;
            Region = region;
            Postal = postal;
            CityName = cityName;
            Organization = organization;
            Latitude = latitude;
            Longitude = longitude;
        }

        /// <summary>
        /// название страны (случайная строка с префиксом "cou_")
        /// </summary>
        public string Country { get; }

        /// <summary>
        /// название области (случайная строка с префиксом "reg_")
        /// </summary>
        public string Region { get; }

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_")
        /// </summary>
        public string Postal { get; }

        /// <summary>
        /// название города (случайная строка с префиксом "cit_")
        /// </summary>
        public string CityName { get; }

        /// <summary>
        /// название организации (случайная строка с префиксом "org_")
        /// </summary>
        public string Organization { get; }

        /// <summary>
        /// широта
        /// </summary>
        public float Latitude { get; }

        /// <summary>
        /// долгота
        /// </summary>
        public float Longitude { get; }
    }
}