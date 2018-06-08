using MetaQuotes.Models.Version2;

namespace MetaQuotes.Services
{
    public interface IRepository
    {
        BinaryGeoBase Db { get; }
        void Load(string filePath);
    }
}