namespace MetaQuotes.Services
{
    public interface IFileReadService
    {
        byte[] ReadAllBytes(string filePath);
    }
}