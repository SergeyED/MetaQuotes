using System;
using System.Collections.Generic;

namespace MetaQuotes.Models
{
    public class GeoBase
    {
        public Header Header { get; set; }
        public IPRange[] Ranges { get; set; }
        public City[] Cities { get; set; }
        public Location[] Locations { get; set; }
    }

}
