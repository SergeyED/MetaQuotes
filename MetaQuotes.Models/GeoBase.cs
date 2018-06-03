namespace MetaQuotes.Models
{
    public class GeoBase
    {
        public Header Header { get; set; }
        public IpRange[] Ranges { get; set; }
        public City[] Cities { get; set; }
        public Location[] Locations { get; set; }
        
        /// <summary>
        /// Данные с логами времени загрузки базы. Требуется закгузка с диска не более 50мс.
        /// </summary>
        public BaseLoadStatistic LoadStatistic { get; set; }

        public GeoBase()
        {
            LoadStatistic = new BaseLoadStatistic();
            
        }
    }
}
