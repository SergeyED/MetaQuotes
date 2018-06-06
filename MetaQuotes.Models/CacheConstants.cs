namespace MetaQuotes.Models
{
    public static class CacheConstants
    {
        public const string GeoBaseKey = "GeoBaseCache";
        public const string BinaryGeoBaseKey = "BinaryGeoBaseKey";

        public static string SearchByIpCacheName(string ip)
        {
            return $"SearchByIP_{ip}";
        }

        public static string SearchByCityCacheName(string city)
        {
            return $"SearchByCity_{city}";
        }
    }
}
