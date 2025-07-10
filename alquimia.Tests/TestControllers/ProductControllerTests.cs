using alquimia.Api.Controllers;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _controller = new ProductController(_mockProductService.Object);
        }

        [Fact]
        public async Task GetPriceRange_ShouldReturnOk_WhenProductFound()
        {
            var noteId = 1;
            var expectedPriceRange = new PriceRangeDTO
            {
                MinPrice = 100,
                MaxPrice = 500
            };

            _mockProductService.Setup(service => service.GetPriceRangeFromProductAsync(noteId))
                .ReturnsAsync(expectedPriceRange);

            var result = await _controller.GetPriceRange(noteId);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedPriceRange = Assert.IsType<PriceRangeDTO>(okResult.Value);
            Assert.Equal(expectedPriceRange.MinPrice, returnedPriceRange.MinPrice);
            Assert.Equal(expectedPriceRange.MaxPrice, returnedPriceRange.MaxPrice);
        }

        [Fact]
        public async Task GetProductsByFormula_ShouldReturnOk_WhenProductsFound()
        {
            var dto = new SearchByFormulaDTO { FormulaId = 1 };
            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1" },
                new ProductDTO { Id = 2, Name = "Product 2" }
            };

            _mockProductService.Setup(service => service.GetProductsByFormulaAsync(dto.FormulaId))
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetProductsByFormula(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<ProductDTO>>(okResult.Value);
            Assert.Equal(expectedProducts.Count, returnedProducts.Count);
        }

        [Fact]
        public async Task GetAllProducts_ShouldReturnOk_WhenProductsFound()
        {
            var expectedProducts = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1" },
                new ProductDTO { Id = 2, Name = "Product 2" }
            };

            _mockProductService.Setup(service => service.GetAllAsync())
                .ReturnsAsync(expectedProducts);

            var result = await _controller.GetProductsByFormula();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<ProductDTO>>(okResult.Value);
            Assert.Equal(expectedProducts.Count, returnedProducts.Count);
        }

        [Fact]
        public async Task GetPriceRange_ShouldReturnNotFound_WhenProductNotFound()
        {
            var noteId = 999;
            _mockProductService.Setup(service => service.GetPriceRangeFromProductAsync(noteId))
                .ReturnsAsync((PriceRangeDTO)null);

            var result = await _controller.GetPriceRange(noteId);

            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task GetProductsByFormula_ShouldReturnOk_WhenNoProductsFound()
        {
            var dto = new SearchByFormulaDTO { FormulaId = 999 };

            _mockProductService.Setup(service => service.GetProductsByFormulaAsync(dto.FormulaId))
                .ReturnsAsync(new List<ProductDTO>());

            var result = await _controller.GetProductsByFormula(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProducts = Assert.IsType<List<ProductDTO>>(okResult.Value);
            Assert.Empty(returnedProducts);
        }
    }
}
