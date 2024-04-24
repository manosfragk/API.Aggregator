using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using API_Aggregator.Services;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace API.Aggregator.Tests.Services
{
    /// <summary>
    /// Unit tests for the IpGeolocationService class.
    /// </summary>
    public class IpGeolocationServiceTests
    {

        private readonly Mock<IMemoryCache> _mockCache;
        private readonly Mock<ILogger<IpGeolocationService>> _mockLogger;

        public IpGeolocationServiceTests()
        {
            _mockCache = new Mock<IMemoryCache>();
            _mockLogger = new Mock<ILogger<IpGeolocationService>>();
        }

        /// <summary>
        /// Tests that GetIpGeolocationAsync successfully retrieves and parses an IP geolocation response.
        /// This test mocks the underlying HttpClient behavior to simulate a successful response.
        /// </summary>
        [Fact]
        public async Task GetIpGeolocationAsync_ReturnsIpGeolocationInfo_OnSuccessfulResponse()
        {
            // Arrange (Mock successful response)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = new StringContent(@"{""ip"": ""8.8.8.8"", ""city"": ""Mountain View""}");
            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var service = new IpGeolocationService(httpClient, _mockCache.Object, _mockLogger.Object);

            // Act
            var ipGeolocationInfo = await service.GetIpGeolocationAsync();

            // Assert
            Assert.NotNull(ipGeolocationInfo);
            Assert.Equal("8.8.8.8", ipGeolocationInfo.Ip);
            Assert.Equal("Mountain View", ipGeolocationInfo.City);
        }

        /// <summary>
        /// Tests that GetIpGeolocationAsync throws an exception when the HTTP request fails.
        /// This test mocks the underlying HttpClient behavior to simulate a failed request.
        /// </summary>
        [Fact]
        public async Task GetIpGeolocationAsync_ThrowsException_OnFailedRequest()
        {
            // Arrange (Mock failed request)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var service = new IpGeolocationService(httpClient, _mockCache.Object, _mockLogger.Object);

            // Act & Assert (expect exception)
            await Assert.ThrowsAsync<Exception>(async () => await service.GetIpGeolocationAsync());
        }
    }
}
