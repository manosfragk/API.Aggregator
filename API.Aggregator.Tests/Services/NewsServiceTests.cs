using API.Aggregator.Services;
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
    /// Unit tests for the NewsService class.
    /// </summary>
    public class NewsServiceTests
    {
        /// <summary>
        /// Tests that GetNewsAsync successfully retrieves and parses news articles from a successful response.
        /// This test mocks the underlying HttpClient behavior to simulate a successful response.
        /// </summary>
        [Fact]
        public async Task GetNewsAsync_ReturnsNewsArticles_OnSuccessfulResponse()
        {
            // Arrange (Mock successful response)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.OK);
            mockResponse.Content = new StringContent(@"{ ""articles"": [ { ""title"": ""Sample Article"" } ] }");
            mockResponse.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeNames.Application.Json);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var newsService = new NewsService(httpClient);

            // Act
            var newsArticles = await newsService.GetNewsAsync("Larisa");

            // Assert
            Assert.NotNull(newsArticles);
            Assert.NotEmpty(newsArticles);
        }

        /// <summary>
        /// Tests that GetNewsAsync throws an exception when encountering an unsuccessful response.
        /// This test mocks the underlying HttpClient behavior to simulate an unsuccessful response.
        /// </summary>
        [Fact]
        public async Task GetNewsAsync_ThrowsException_OnUnsuccessfulResponse()
        {
            // Arrange (Mock unsuccessful response)
            var mockResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError);

            var httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(mockResponse);

            var httpClient = new HttpClient(httpMessageHandlerMock.Object);
            var newsService = new NewsService(httpClient);

            // Act & Assert (expect exception)
            await Assert.ThrowsAsync<HttpRequestException>(async () => await newsService.GetNewsAsync("Larisa"));
        }
    }
}
