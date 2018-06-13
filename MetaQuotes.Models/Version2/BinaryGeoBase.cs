namespace MetaQuotes.Models.Version2
{
    public interface IBinaryGeoBase{
        Header Header { get; }
        uint[,] IpRanges { get; }
        byte[,] Cities { get; }
        int[] Locations { get; }

        /// <summary>
        /// Данные с логами времени загрузки базы. Требуется закгузка с диска не более 50мс.
        /// </summary>
        BaseLoadStatistic LoadStatistic { get; set; }
    }
    public class BinaryGeoBase: IBinaryGeoBase
    {
        public BinaryGeoBase(){
            
        }

        public BinaryGeoBase(Header header, uint[,] ipRanges, byte[,] cities, int[] locations)
        {
            Header = header;
            IpRanges = ipRanges;
            Cities = cities;
            Locations = locations;
        }

        public Header Header { get; }
        public uint[,] IpRanges { get; }
        public byte[,] Cities { get; }
        public int[] Locations { get; }

        /// <summary>
        /// Данные с логами времени загрузки базы. Требуется закгузка с диска не более 50мс.
        /// </summary>
        public BaseLoadStatistic LoadStatistic { get; set; }
    }
}
