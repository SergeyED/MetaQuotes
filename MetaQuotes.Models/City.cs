using System.Runtime.InteropServices;

namespace MetaQuotes.Models
{
    [StructLayout(LayoutKind.Sequential, Size = Constants.LocationsSize)]
    public struct City
    {
        /// <summary>
        /// название страны (случайная строка с префиксом "cou_")
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 8)]
        public string country;      

        /// <summary>
        /// название области (случайная строка с префиксом "reg_")
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string region;        

        /// <summary>
        /// почтовый индекс (случайная строка с префиксом "pos_")
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 12)]
        public string postal;       

        /// <summary>
        /// название города (случайная строка с префиксом "cit_")
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 24)]
        public string city;       

        /// <summary>
        /// название организации (случайная строка с префиксом "org_")
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string organization; 

        /// <summary>
        /// широта
        /// </summary>
        public float latitude;

        /// <summary>
        /// долгота
        /// </summary>
        public float longitude; 

    }


}
