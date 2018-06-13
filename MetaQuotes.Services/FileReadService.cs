using System.IO;

namespace MetaQuotes.Services
{
    public class FileReadService : IFileReadService
    {
        public FileReadService()
        {
        }

        public byte[] ReadAllBytes(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new FileNotFoundException("Не указано имя файла");

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Файл не существует по указанному пути: {filePath}");

            var file = File.ReadAllBytes(filePath);
            return file;
        }
    }
}