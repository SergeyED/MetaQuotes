using System;
using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using MetaQuotes.Models.Version2;

namespace MetaQuotes.Services.Tests
{

    public class SearchServiceTests
    {
        Mock<ISearchService> searchServiceMock;
        ISearchService searchService;
        Mock<IMemoryCache> memoreCacheMock;
        Mock<IConverterService> converterService;
        Mock<Repository> repositoryMock;
        Mock<IFileReadService> fileReadService;

        BinaryGeoBase db;

        public SearchServiceTests()
        {
            searchServiceMock = new Mock<ISearchService>();
            memoreCacheMock = new Mock<IMemoryCache>();
            converterService = new Mock<IConverterService>();
            repositoryMock = new Mock<Repository>();
            fileReadService = new Mock<IFileReadService>();
        }

        [Fact]
        public void CheckHeader(){
            var binary = fileReadService.Object.ReadAllBytes(String.Empty);

        }



        [Fact]
        public void BinarySearchByIpAddressTest()
        {

        }
    }
}
