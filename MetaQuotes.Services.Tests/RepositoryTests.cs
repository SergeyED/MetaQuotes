using Moq;
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

            converterServiceMock = new Mock<IConverterService>();

            fileReadServiceMock = new Mock<IFileReadService>();
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            fileReadServiceMock.Setup(x => x.ReadAllBytes("secret")).Returns(binary);
        }

        [Fact]
        public void LoadDbToSingletoneObjectTest(){
            var repository = new Repository(fileReadServiceMock.Object, new ConverterService());

            repository.Load("secret");

            Assert.NotNull(repository.Db);
        }
    }
}
