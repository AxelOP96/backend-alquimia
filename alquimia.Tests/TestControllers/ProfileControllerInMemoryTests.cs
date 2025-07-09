using alquimia.Api.Controllers;
using alquimia.Data;
using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class ProfileControllerInMemoryTests
    {
        private AlquimiaDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // DB única por test
                .Options;

            return new AlquimiaDbContext(options);
        }

        private UserManager<User> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            userManager.Setup(um => um.GetUserId(It.IsAny<ClaimsPrincipal>()))
                       .Returns((ClaimsPrincipal cp) =>
                       {
                           return cp.FindFirstValue(ClaimTypes.NameIdentifier);
                       });

            return userManager.Object;
        }

        private IHttpContextAccessor GetHttpContextAccessor(int userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString())
            }, "mock"));

            var context = new DefaultHttpContext { User = user };

            var mockAccessor = new Mock<IHttpContextAccessor>();
            mockAccessor.Setup(a => a.HttpContext).Returns(context);

            return mockAccessor.Object;
        }

        [Fact]
        public async Task GetMyData_UserFound_ReturnsOkWithUserProfile()
        {
            // Arrange
            var context = GetDbContext();
            var user = new User
            {
                Id = 1,
                UserName = "user1",
                Email = "user1@example.com",
                Name = "User One",
                EsProveedor = true,
                Cuil = "20-12345678-9",
                Empresa = "EmpresaTest",
                Rubro = "Cosmética"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(user.Id);

            var service = new ProfileService(userManager, context, httpContextAccessor);
            var controller = new ProfileController(service);

            // Act
            var result = await controller.GetMyData();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Equal("User One", profile.Name);
            Assert.Equal("user1@example.com", profile.Email);
        }

        [Fact]
        public async Task GetMyData_UserNotFound_ReturnsNotFound()
        {
            // Arrange
            var context = GetDbContext();
            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(999); // ID inexistente

            var service = new ProfileService(userManager, context, httpContextAccessor);
            var controller = new ProfileController(service);

            // Act
            var result = await controller.GetMyData();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Usuario no encontrado", notFoundResult.Value);
        }
    }
}
