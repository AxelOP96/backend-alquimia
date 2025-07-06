using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using alquimia.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
            // Arrange
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Producto Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo 1"
            };

            var productType = new ProductType { Description = "Tipo 1" };
            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.CreateProductAsync(createProductDTO, 1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createProductDTO.Name, result.Name);
            Assert.Equal(createProductDTO.Description, result.Description);
        }

        [Fact]
        public async Task DeleteProductAsync_ShouldReturnTrue_WhenProductIsDeleted()
        {
            // Arrange
            var product = new Product { Name = "Producto A", Description = "Descripción A", IdProveedor = 1 };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _productService.DeleteProductAsync(product.Id, 1);

            // Assert
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
            // Arrange
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Perfume Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo Inexistente" 
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.CreateProductAsync(createProductDTO, 1));

            Assert.Equal("Tipo de producto no válido", exception.Message);
        }


        [Fact]
        public async Task GetProductsByProviderAsync_ShouldReturnEmptyList_WhenProviderHasNoProducts()
        {
            // Arrange
            var providerId = 30; 

            // Act
            var result = await _productService.GetProductsByProviderAsync(providerId);

            // Assert
            Assert.Empty(result);
        }
        [Fact]
        public async Task CreateProductAsync_ShouldThrowException_WhenProductTypeIsInvalidNew()
        {
            // Arrange
            var createProductDTO = new CreateProductoDTO
            {
                Name = "Perfume Nuevo",
                Description = "Descripción Producto Nuevo",
                TipoProductoDescription = "Tipo Inexistente" 
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.CreateProductAsync(createProductDTO, 1));

            Assert.Equal("Tipo de producto no válido", exception.Message);
        }

        

        [Fact]
        public async Task UpdateProductAsync_ShouldThrowException_WhenProductNotFound()
        {
            // Arrange
            var updateDTO = new UpdateProductoDTO { Name = "Perfume Actualizado" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _productService.UpdateProductAsync(999, updateDTO, 1)); 

            Assert.Equal("Producto no encontrado o no pertenece al proveedor", exception.Message);
        }

        

        //[Fact]
        //public async Task AddVariantsToProductAsync_ShouldAddVariant_WhenProductExists()
        //{
        //    // Arrange
        //    var product = new Product
        //    {
        //        Name = "Perfume A",
        //        Description = "Perfume de prueba",
        //        IdProveedor = 1
        //    };

        //    _context.Products.Add(product);
        //    await _context.SaveChangesAsync();

        //    var variantDTO = new CreateProductVariantDTO
        //    {
        //        Volume = 50,
        //        Unit = "ml",
        //        Price = 30.0M,
        //        Stock = 20,
        //        IsHypoallergenic = true
        //    };

        //    // Act
        //    await _productService.AddVariantsToProductAsync(product.Id, variantDTO);

        //    // Assert
        //    var variant = _context.ProductVariants.FirstOrDefault();
        //    Assert.NotNull(variant);
        //    Assert.Equal(50, variant.Volume);
        //    Assert.Equal("ml", variant.Unit);
        //}

        [Fact]
        public async Task DeleteVariantAsync_ShouldReturnTrue_WhenVariantIsDeleted()
        {
            // Arrange
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

            // Act
            var result = await _productService.DeleteVariantAsync(variant.Id);

            // Assert
            Assert.True(result);
            Assert.Null(_context.ProductVariants.Find(variant.Id));
        }

        [Fact]
        public async Task DecreaseVariantStockAsync_ShouldDecreaseStock_WhenEnoughStock()
        {
            var variant = new ProductVariant
            {
                Volume = 50,
                Unit = "ml",
                Price = 30m,
                Stock = 5
            };

            _context.ProductVariants.Add(variant);
            await _context.SaveChangesAsync();

            await _productService.DecreaseVariantStockAsync(variant.Id, 3);

            var updated = await _context.ProductVariants.FindAsync(variant.Id);
            Assert.Equal(2, updated.Stock);
        }

    }
}
