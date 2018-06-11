using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using System.IO;

namespace MetaQuotes.Services.Tests
{
    public class RepositoryTests{
        Mock<IRepository> repositoryMock;

        public RepositoryTests(){
            repositoryMock = new Mock<IRepository>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("ssss")]
        public void LoadWithUncorrectFileName(string fileName){

            var repository = new Repository();
            Assert.Throws<FileNotFoundException>(() => repository.Load(fileName));
        }
    }

    public class SearchServiceTests
    {
        Mock<ISearchService> searchServiceMock;
        Mock<IMemoryCache> memoreCacheMock;
        Mock<IConverterService> converterService;
        Mock<IRepository> repositoryMock;

        public SearchServiceTests()
        {
            searchServiceMock = new Mock<ISearchService>();
            memoreCacheMock = new Mock<IMemoryCache>();
            converterService = new Mock<IConverterService>();
            repositoryMock = new Mock<IRepository>();

            repositoryMock.Setup(x => x.Load(string.Empty));
        }

        [Fact]
        public void BinarySearchByIpAddressTest()
        {

        }
    }
}
