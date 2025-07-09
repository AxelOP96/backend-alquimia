using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using Xunit;

public class MercadoLibreServiceTests
{
    [Fact]
    public async Task SyncProductsAsync_CreatesProducts()
    {
        var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
            .UseInMemoryDatabase("meli-sync")
            .Options;
        using var context = new AlquimiaDbContext(options);
        var productServiceMock = new Mock<IProductService>();

        var handler = new QueueMessageHandler(new[]
        {
            new HttpResponseMessage(HttpStatusCode.OK){ Content = new StringContent("{\"id\":1}") },
            new HttpResponseMessage(HttpStatusCode.OK){ Content = new StringContent("{\"results\":[\"IT1\"]}") },
            new HttpResponseMessage(HttpStatusCode.OK){ Content = new StringContent("{\"title\":\"Prod\",\"price\":10,\"available_quantity\":5}") }
        });
        var httpClient = new HttpClient(handler);
        var factoryMock = new Mock<IHttpClientFactory>();
        factoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var settings = Microsoft.Extensions.Options.Options.Create(new MercadoLibreSettings { AppId = "id", ClientSecret = "sec" });
        var service = new MercadoLibreService(productServiceMock.Object, factoryMock.Object, context, settings);

        await service.SyncProductsAsync(2, "token");

        productServiceMock.Verify(p => p.CreateProductAsync(It.IsAny<CreateProductoDTO>(), 2), Times.Once);
    }

  

    private class QueueMessageHandler : HttpMessageHandler
    {
        private readonly Queue<HttpResponseMessage> _responses;
        public QueueMessageHandler(IEnumerable<HttpResponseMessage> responses)
        {
            _responses = new Queue<HttpResponseMessage>(responses);
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            return Task.FromResult(_responses.Dequeue());
        }
    }
}
