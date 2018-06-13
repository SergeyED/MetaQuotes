using Xunit;
using Moq;
using Microsoft.Extensions.Caching.Memory;
using MetaQuotes.Services.Common;
using MetaQuotes.Services.Exceptions;

namespace MetaQuotes.Services.Tests
{
    public class SearchServiceTests
    {
        Mock<IMemoryCache> memoryCacheMock;
        Mock<IFileReadService> fileReadServiceMock;
        Mock<ISearchService> searchServiceMock;

        IRepository repository;

        public SearchServiceTests()
        {
            memoryCacheMock = new Mock<IMemoryCache>();
            searchServiceMock = new Mock<ISearchService>();

            fileReadServiceMock = new Mock<IFileReadService>();
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            fileReadServiceMock.Setup(x => x.ReadAllBytes("secret")).Returns(binary);

            repository = new Repository(fileReadServiceMock.Object, new ConverterService());
            repository.Load("secret");
        }

        [Fact]
        public void BinarySearchByCorrectIpAddressTest()
        {
            var ipString = IpAddressHelpers.IpUintToString(9);
            var searchService = new SearchService(new ConverterService(), repository);

            var result = searchService.BinarySearchByIpAddress(ipString);

            Assert.Equal(result.Length, 1);
            Assert.Equal(result[0].CityName, "cit_1");
        }

        [Fact]
        public void BinarySearchByIpAddressOutsideDbTest()
        {
            var ipString = IpAddressHelpers.IpUintToString(7);
            var searchService = new SearchService(new ConverterService(), repository);

            var result = searchService.BinarySearchByIpAddress(ipString);

            Assert.Equal(result.Length, 0);
        }

        [Fact]
        public void BinarySearchByUncorrectIpAddressTest()
        {
            var ipString = "1.1.1.1.1";
            var searchService = new SearchService(new ConverterService(), repository);

            Assert.Throws<IpAddressConvertToUIntException>(() => searchService.BinarySearchByIpAddress(ipString));
        }

        [Fact]
        public void SearchByCityNameWithResultTest(){
            var cityName = "cit_1";
            var searchService = new SearchService(new ConverterService(), repository);

            var result = searchService.BinarySearchByCityName(cityName);

            Assert.Equal(result.Length, 1);
            Assert.Equal(result[0].CityName, cityName);
        }

        [Fact]
        public void SearchByCityNameWithoutResultTest()
        {
            var cityName = "cit_12";
            var searchService = new SearchService(new ConverterService(), repository);

            var result = searchService.BinarySearchByCityName(cityName);

            Assert.Empty(result);
        }
    }
}
