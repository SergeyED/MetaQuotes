using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public interface IBinaryLoader
    {
        GeoBase LoadDb(string filePath);
    }
}
