using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    public class City
    {
        /// <summary>
        /// название страны (случайная строка с префиксом "cou_")
        /// </summary>
        public string Country { get; set; }      

        /// <summary>
        /// название области (случайная строка с префиксом "reg_")
        /// </summary>
        public string Region { get; set; }        

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_")
        /// </summary>
        public string Postal { get; set; }       

        /// <summary>
        /// название города (случайная строка с префиксом "cit_")
        /// </summary>
        public string CityName { get; set; }       

        /// <summary>
        /// название организации (случайная строка с префиксом "org_")
        /// </summary>
        public string Organization { get; set; }

        /// <summary>
        /// широта
        /// </summary>
        public float Latitude { get; set; }

        /// <summary>
        /// долгота
        /// </summary>
        public float Longitude { get; set; } 

    }


}
