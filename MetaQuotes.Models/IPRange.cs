namespace MetaQuotes.Models
{
    public struct IPRange
    {
        /// <summary>
        /// начало диапазона IP адресов
        /// </summary>
        public uint ip_from;

        /// <summary>
        /// индекс записи о местоположении
        /// </summary>
        public uint location_index;

        /// <summary>
        /// конец диапазона IP адресов
        /// </summary>
        public uint ip_to;
    }


}
