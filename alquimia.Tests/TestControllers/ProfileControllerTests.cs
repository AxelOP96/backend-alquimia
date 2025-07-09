using alquimia.Api.Controllers;
using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class ProfileControllerTests
    {
        private readonly Mock<IProfileService> _mockService;
        private readonly ProfileController _controller;

        public ProfileControllerTests()
        {
            _mockService = new Mock<IProfileService>();
            _controller = new ProfileController(_mockService.Object);
        }

        [Fact]
        public async Task GetMyData_UserNotFound_ReturnsNotFound()
        {
            _mockService.Setup(s => s.BringMyData()).ReturnsAsync((UserProfileDto)null);
            var result = await _controller.GetMyData();
            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado", actionResult.Value);
        }

        [Fact]
        public async Task GetMyData_UserFound_ReturnsOk()
        {
            var userProfile = new UserProfileDto { Name = "Juan" };
            _mockService.Setup(service => service.BringMyData()).ReturnsAsync(userProfile);

            var result = await _controller.GetMyData();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserProfileDto>(actionResult.Value);
            Assert.Equal("Juan", returnValue.Name);
        }

        [Fact]
        public async Task GetMyData_UserFound_ReturnsOkWithUserData()
        {
            var user = new UserProfileDto { Name = "Juan", Email = "juan@mail.com" };
            _mockService.Setup(s => s.BringMyData()).ReturnsAsync(user);

            var result = await _controller.GetMyData();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<UserProfileDto>(actionResult.Value);
            Assert.Equal("Juan", returnedUser.Name);
            Assert.Equal("juan@mail.com", returnedUser.Email);
        }

        [Fact]
        public async Task GetMyFormulas_UserHasNoFormulas_ReturnsEmptyList()
        {
            _mockService.Setup(s => s.BringMyFormulasAsync()).ReturnsAsync(new List<Formula>());

            var result = await _controller.GetMyFormulas();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Formula>>(actionResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetMyFormulas_UserHasFormulas_ReturnsList()
        {
            var formulas = new List<Formula> { new Formula { Id = 1, Title = "Formula 1" } };
            _mockService.Setup(s => s.BringMyFormulasAsync()).ReturnsAsync(formulas);

            var result = await _controller.GetMyFormulas();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Formula>>(actionResult.Value);
            Assert.Single(list);
            Assert.Equal("Formula 1", list[0].Title);
        }

        [Fact]
        public async Task GetMyProducts_UserHasNoProducts_ReturnsEmptyList()
        {
            _mockService.Setup(s => s.BringMyProducts()).ReturnsAsync(new List<Product>());

            var result = await _controller.GetMyProducts();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Product>>(actionResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetMyProducts_UserHasProducts_ReturnsList()
        {
            var products = new List<Product> { new Product { Id = 1, Name = "Product 1" } };
            _mockService.Setup(s => s.BringMyProducts()).ReturnsAsync(products);

            var result = await _controller.GetMyProducts();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<Product>>(actionResult.Value);
            Assert.Single(list);
            Assert.Equal("Product 1", list[0].Name);
        }

        [Fact]
        public async Task GetMyWishlist_UserHasNoWishlist_ReturnsEmptyList()
        {
            var userId = "123";
            _mockService.Setup(s => s.GetUserWishlistAsync(userId)).ReturnsAsync(new List<ProductDTO>());

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            };

            var result = await _controller.GetWishlist();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<ProductDTO>>(actionResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public async Task GetMyWishlist_UserHasWishlist_ReturnsList()
        {
            var userId = "123";
            var wishlist = new List<ProductDTO> { new ProductDTO { Id = 2, Name = "WishProduct" } };
            _mockService.Setup(s => s.GetUserWishlistAsync(userId)).ReturnsAsync(wishlist);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, userId)
                    }))
                }
            };

            var result = await _controller.GetWishlist();

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<ProductDTO>>(actionResult.Value);
            Assert.Single(list);
            Assert.Equal("WishProduct", list[0].Name);
        }

        [Fact]
        public async Task UpdateMyData_UserNotFound_ReturnsNotFound()
        {
            var dto = new UserProfileUpdateDto { Name = "Juan" };
            _mockService.Setup(s => s.UpdateMyData(dto)).ReturnsAsync((UserProfileDto)null);

            var result = await _controller.UpdateMyData(dto);

            var actionResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado", actionResult.Value);
        }

        [Fact]
        public async Task UpdateMyData_UserUpdated_ReturnsOkWithUpdatedData()
        {
            var dto = new UserProfileUpdateDto { Name = "Juan" };
            var updatedDto = new UserProfileDto { Name = "Juan", Email = "juan@mail.com" };
            _mockService.Setup(s => s.UpdateMyData(dto)).ReturnsAsync(updatedDto);

            var result = await _controller.UpdateMyData(dto);

            var actionResult = Assert.IsType<OkObjectResult>(result);
            var returnedDto = Assert.IsType<UserProfileDto>(actionResult.Value);
            Assert.Equal("Juan", returnedDto.Name);
            Assert.Equal("juan@mail.com", returnedDto.Email);
        }
    }
}