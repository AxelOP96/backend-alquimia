using Alquimia.Api.Controllers;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;

public class MercadoLibreControllerTests
{
    [Fact]
    public async Task OrderNotification_ReturnsOk()
    {
        var dto = new MercadoLibreOrderDTO { VariantId = 1, Quantity = 2 };
        var mockService = new Mock<IMercadoLibreService>();
        var controller = new MercadoLibreController(mockService.Object);

        var result = await controller.OrderNotification(dto);

        Assert.IsType<OkResult>(result);
        mockService.Verify(s => s.ProcessOrderAsync(dto), Times.Once);
    }

    [Fact]
    public async Task OrderNotification_ReturnsNotFound_OnKeyNotFound()
    {
        var dto = new MercadoLibreOrderDTO { VariantId = 1, Quantity = 2 };
        var mockService = new Mock<IMercadoLibreService>();
        mockService.Setup(s => s.ProcessOrderAsync(dto))
            .ThrowsAsync(new KeyNotFoundException("No encontrado"));
        var controller = new MercadoLibreController(mockService.Object);

        var result = await controller.OrderNotification(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("No encontrado", notFound.Value);
    }

    [Fact]
    public async Task OrderNotification_ReturnsBadRequest_OnInvalidOperation()
    {
        var dto = new MercadoLibreOrderDTO { VariantId = 1, Quantity = 2 };
        var mockService = new Mock<IMercadoLibreService>();
        mockService.Setup(s => s.ProcessOrderAsync(dto))
            .ThrowsAsync(new InvalidOperationException("sin stock"));
        var controller = new MercadoLibreController(mockService.Object);

        var result = await controller.OrderNotification(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("sin stock", badRequest.Value);
    }
}
