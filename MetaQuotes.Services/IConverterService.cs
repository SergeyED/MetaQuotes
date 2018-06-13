using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public interface IConverterService
    {
        City ConvertToCity(byte[] source, int offset);
        Header ConvertToHeader(byte[] file, int offset);
    }
}