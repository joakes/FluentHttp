using Moq;
using RichardSzalay.MockHttp;
using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.Logging;

namespace ScratchConsole.Test
{
    public class CarClientTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new();
        private readonly MockHttpMessageHandler _handlerMock = new();
        private readonly Mock<ILogger<CarClient>> _logger;
        private readonly CarClient _sut;

        public CarClientTests()
        {
            _httpClientFactory.Setup(x => x.CreateClient("CarClient"))
                .Returns(new HttpClient(_handlerMock)
                {
                    BaseAddress = new Uri("https://api.cars.com/")
                });

            var httpClient = _httpClientFactory.Object.CreateClient("CarClient");
            var logger = new Mock<ILogger<CarClient>>();

            _sut = new CarClient(httpClient, logger.Object);
        }

        [Fact]
        public async Task FindCarAsync_CanFindCar()
        {
            // arrange
            _handlerMock.When(HttpMethod.Get, "https://api.cars.com/car?name=123")
                .Respond(HttpStatusCode.OK, JsonContent.Create(new
                {
                    id = "123",
                    make = "Ford",
                    model = "Falcon"
                }));

            // act
            var httpResult = await _sut.FindCarAsync("123");

            // assert
            httpResult.IsSuccess.Should().BeTrue();
            httpResult.StatusCode.Should().Be(200);
            
            var value = httpResult.Value;
            value.Should().NotBeNull();
            value.Id.Should().Be("123");
            value.Make.Should().Be("Ford");
            value.Model.Should().Be("Falcon");
        }
    }
}