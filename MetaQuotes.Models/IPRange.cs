namespace MetaQuotes.Models
{
    public class IpRange
    {
        public IpRange(uint ipFrom, uint ipTo, uint locationIndex)
        {
            IpFrom = ipFrom;
            IpTo = ipTo;
            LocationIndex = locationIndex;
        }

        public uint IpFrom { get; }
        public uint IpTo { get; }
        public uint LocationIndex { get;  }
    }

}
