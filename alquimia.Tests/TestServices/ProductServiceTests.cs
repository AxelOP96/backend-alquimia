using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class ProductServiceTests
    {
        private readonly AlquimiaDbContext _context;
        private readonly ProductService _productService;
        private readonly Mock<IFormulaService> _formulaServiceMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        public ProductServiceTests()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase(databaseName: "AlquimiaTestDb")
                .Options;

            _context = new AlquimiaDbContext(options);
            _userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var mockFormulaService = new Mock<IFormulaService>();
            _productService = new ProductService(_context, mockFormulaService.Object);
        }
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }


        [Fact]
        public async Task CreateProductAsync_ShouldReturnProduct_WhenProductIsCreated()
        {
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Producto Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo 1"
            };

            var productType = new ProductType { Description = "Tipo 1" };
            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            var result = await _productService.CreateProductAsync(createProductDTO, 1);

            Assert.NotNull(result);
            Assert.Equal(createProductDTO.Name, result.Name);
            Assert.Equal(createProductDTO.Description, result.Description);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnTrue_WhenProductIsDeleted()
        {
            var product = new Product { Name = "Producto A", Description = "Descripción A", IdProveedor = 1 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var result = await _productService.DeleteProductAsync(product.Id, 1);

            Assert.True(result);
            Assert.Null(_context.Products.Find(product.Id));
        }

        [Fact]
        public async Task GetProductByIdAsync_ShouldThrowException_WhenProductNotFound()
        {
            var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.GetProductByIdAsync(999, 1));

            Assert.Equal("Producto no encontrado", result.Message);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldCreateProduct_WhenValidDataIsProvided()
        {
            var dto = new CreateProductoDTO
            {
                Name = "Perfume Nuevo",
                Description = "Nuevo perfume",
                TipoProductoDescription = "Perfumes",
                Variants = new List<CreateProductVariantDTO>
        {
            new CreateProductVariantDTO { Volume = 100, Unit = "ml", Price = 50.0M, Stock = 10, IsHypoallergenic = true }
        }
            };

            _context.ProductTypes.Add(new ProductType { Description = "Perfumes" });
            await _context.SaveChangesAsync();

            var result = await _productService.CreateProductAsync(dto, 1);

            Assert.NotNull(result);
            Assert.Equal("Perfume Nuevo", result.Name);
        }


        [Fact]
        public async Task DeleteProductAsync_ShouldReturnFalse_WhenProductNotFound()
        {
            var result = await _productService.DeleteProductAsync(999, 8);
            Assert.False(result);
        }

        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenProductTypeIsInvalid()
        {
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Perfume Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo Inexistente"
            };

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.CreateProductAsync(createProductDTO, 1));

            Assert.Equal("Tipo de producto no válido", exception.Message);
        }

        [Fact]
        public async Task GetProductsByProviderAsync_ShouldReturnEmptyList_WhenProviderHasNoProducts()
        {
            var providerId = 30;
            var result = await _productService.GetProductsByProviderAsync(providerId);
            Assert.Empty(result);
        }
        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenProductTypeIsInvalidNew()
        {
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Perfume Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo Inexistente"
            };
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.CreateProductAsync(createProductDTO, 1));

            Assert.Equal("Tipo de producto no válido", exception.Message);
        }

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            var updateDTO = new UpdateProductoDTO { Name = "Perfume Actualizado" };

            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.UpdateProductAsync(999, updateDTO, 1));

            Assert.Equal("Producto no encontrado o no pertenece al proveedor", exception.Message);
        }

        [Fact]
        public async Task DeleteVariantAsync_ShouldReturnTrue_WhenVariantIsDeleted()
        {
            var variant = new ProductVariant
            {
                Volume = 100,
                Unit = "ml",
                Price = 50.0M,
                Stock = 10,
                IsHypoallergenic = true
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            var result = await _productService.DeleteVariantAsync(variant.Id);

            Assert.True(result);
            Assert.Null(_context.ProductVariants.Find(variant.Id));
        }
    }
}
