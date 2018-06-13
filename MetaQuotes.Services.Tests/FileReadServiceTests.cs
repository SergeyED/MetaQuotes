using Xunit;
using Moq;
using System.IO;
using System;
using MetaQuotes.Models;

namespace MetaQuotes.Services.Tests
{
    public class FileReadServiceTests
    {
        private readonly Mock<IFileReadService> fileReadServiceMock;

        public FileReadServiceTests()
        {
            fileReadServiceMock = new Mock<IFileReadService>();

            var binary = GeoBaseTestData.GetBinaryGeoBase();

            fileReadServiceMock.Setup(x => x.ReadAllBytes("secret")).Returns(binary);
        }


        [Theory]
        [InlineData("")]
        [InlineData("ssss")]
        public void LoadWithUncorrectFileNameTest(string fileName)
        {
            var fileReadService = new FileReadService();

            Assert.Throws<FileNotFoundException>(() => fileReadService.ReadAllBytes(fileName));
        }

        [Fact]
        public void ReadAndCheckDataTest(){
            var binary = fileReadServiceMock.Object.ReadAllBytes("secret");
            var converter = new ConverterService();

            var header = converter.ConvertToHeader(binary, 0);
            var firstIpFrom = BitConverter.ToInt32(binary, (int)header.OffsetRanges);
            var firstCity = converter.ConvertToCity(binary, (int)header.OffsetCities);
            var secondIndex = BitConverter.ToInt32(binary, (int)header.OffsetLocations + Constants.IndexSize);

            Assert.Equal(header.Name, GeoBaseTestData.DbName);
            Assert.Equal<int>(8, firstIpFrom);
            Assert.Equal("cit_1", firstCity.CityName);
            Assert.Equal(1, secondIndex);
        }
    }
}