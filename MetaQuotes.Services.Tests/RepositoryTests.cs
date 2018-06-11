using Xunit;
using Moq;
using System.IO;

namespace MetaQuotes.Services.Tests
{
    public class RepositoryTests
    {
        Mock<IRepository> repositoryMock;

        public RepositoryTests()
        {
            repositoryMock = new Mock<IRepository>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ssss")]
        public void LoadWithUncorrectFileName(string fileName)
        {

            var repository = new Repository();
            Assert.Throws<FileNotFoundException>(() => repository.Load(fileName));
        }
    }
}
