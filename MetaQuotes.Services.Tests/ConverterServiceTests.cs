using Xunit;
using Moq;
using System;
using MetaQuotes.Models;

namespace MetaQuotes.Services.Tests
{
    public class ConverterServiceTests
    {
        private readonly Mock<IConverterService> converterServiceMock;

        public ConverterServiceTests()
        {
            converterServiceMock = new Mock<IConverterService>();
        }

        [Fact]
        public void ConvertCorrectBinaryToHeader()
        {
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            var converter = new ConverterService();

            var header = converter.ConvertToHeader(binary, 0);

            Assert.Equal(header.Version, 1);
            Assert.Equal(header.Name, GeoBaseTestData.DbName);
        }

        [Fact]
        public void ConvertCorrectBinaryToCity()
        {
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            var converter = new ConverterService();

            var header = converter.ConvertToHeader(binary, 0);
            var firstCity = converter.ConvertToCity(binary, (int)header.OffsetCities);
            var secondCity = converter.ConvertToCity(binary, (int)header.OffsetCities + Constants.CitySize);

            Assert.Equal(firstCity.CityName, "cit_1");
            Assert.Equal(secondCity.CityName, "cit_2");        
        }

        [Fact]
        public void ExceptionWithOffsetGreatedSizeTest()
        {
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            var converter = new ConverterService();

            Assert.Throws<ArgumentOutOfRangeException>(() => converter.ConvertToCity(binary, binary.Length + 1));
            Assert.Throws<ArgumentOutOfRangeException>(() => converter.ConvertToHeader(binary, binary.Length + 1));
        }

        [Fact]
        public void ExceptionWithOffsetGreatedObjectSizeTest()
        {
            var binary = GeoBaseTestData.GetBinaryGeoBase();
            var converter = new ConverterService();

            Assert.Throws<ArgumentOutOfRangeException>(() => converter.ConvertToCity(binary, binary.Length - 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => converter.ConvertToHeader(binary, binary.Length - 10));
        }
    }
}
