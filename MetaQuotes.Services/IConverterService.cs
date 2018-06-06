using MetaQuotes.Models;

namespace MetaQuotes.Services
{
    public interface IConverterService
    {
        City ConvertToCity(byte[] source, int offset);
    }

}