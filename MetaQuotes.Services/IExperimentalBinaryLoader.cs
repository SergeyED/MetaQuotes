using MetaQuotes.Models.Version2;

namespace MetaQuotes.Services
{
    public interface IExperimentalBinaryLoader
    {
        BinaryGeoBase ReadBinaryFileToByteArray(string filePath);
    }
}