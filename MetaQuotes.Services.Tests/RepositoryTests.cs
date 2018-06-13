using Moq;
using System.IO;
using Xunit;

namespace MetaQuotes.Services.Tests
{
    public class RepositoryTests
    {
        Mock<IRepository> repositoryMock;
        Mock<IFileReadService> fileReadServiceMock;
        Mock<IConverterService> converterServiceMock;

        public RepositoryTests()
        {
            repositoryMock = new Mock<IRepository>();
            fileReadServiceMock = new Mock<IFileReadService>();
            converterServiceMock = new Mock<IConverterService>();
        }


    }
}
