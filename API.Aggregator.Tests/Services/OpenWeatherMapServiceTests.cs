using API_Aggregator.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Moq.Protected;

namespace API.Aggregator.Tests.Services
{
    /// <summary>
    /// Unit tests for the OpenWeatherMapService class.
    /// </summary>
    public class OpenWeatherMapServiceTests
    {
        /// <summary>
        /// Tests that GetWeatherAsync successfully retrieves and parses weather information from a successful response.
        /// This test mocks the underlying HttpClient behavior to simulate a successful response.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_ReturnsWeatherInfo_OnSuccessfulResponse()
        {
            // Arrange (Mock successful response)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = new StringContent(@"{ ""main"": { ""temp"": 25 }, ""name"": ""Athens"" }");
            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var weatherService = new OpenWeatherMapService(httpClient);

            // Act
            var weatherInfo = await weatherService.GetWeatherAsync("Athens");

            // Assert
            Assert.NotNull(weatherInfo);
        }

        /// <summary>
        /// Tests that GetWeatherAsync throws an exception when encountering an unsuccessful response.
        /// This test mocks the underlying HttpClient behavior to simulate an unsuccessful response.
        /// </summary>
        [Fact]
        public async Task GetWeatherAsync_ThrowsException_OnUnsuccessfulResponse()
        {
            // Arrange (Mock unsuccessful response)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.NotFound);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var weatherService = new OpenWeatherMapService(httpClient);

            // Act & Assert (expect exception)
            await Assert.ThrowsAsync<HttpRequestException>(async () => await weatherService.GetWeatherAsync("Athens"));
        }
    }
}
