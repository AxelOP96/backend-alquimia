using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class ProfileServiceTests
    {
        private AlquimiaDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase(databaseName: "AlquimiaTestDb")
                .Options;

            var context = new AlquimiaDbContext(options);

            return context;
        }

        private UserManager<User> GetUserManagerMock()
        {
            var store = new Mock<IUserStore<User>>();
            return new UserManager<User>(store.Object, null, null, null, null, null, null, null, null);
        }

        private IHttpContextAccessor GetHttpContextAccessor(int userId)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            }, "mock"));

            var httpContext = new DefaultHttpContext() { User = user };
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(_ => _.HttpContext).Returns(httpContext);

            return mockHttpContextAccessor.Object;
        }

        [Fact]
        public async Task BringMyData_UserExists_ReturnsUserProfileDto()
        {
            var context = GetDbContext();

            var user = new User
            {
                Id = 1,
                UserName = "testuser",
                Email = "testuser@example.com",
                Name = "Test User",
                EsProveedor = false
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userManager = GetUserManagerMock();

            var httpContextAccessor = GetHttpContextAccessor(user.Id);

            var service = new ProfileService(userManager, context, httpContextAccessor);

            var result = await service.BringMyData();

            Assert.NotNull(result);
            Assert.Equal("Test User", result.Name);
            Assert.Equal("testuser@example.com", result.Email);
            Assert.False(result.EsProveedor);
        }

        [Fact]
        public async Task BringMyData_UserDoesNotExist_ReturnsNull()
        {
            var context = GetDbContext();

            var userManager = GetUserManagerMock();

            var httpContextAccessor = GetHttpContextAccessor(999);

            var service = new ProfileService(userManager, context, httpContextAccessor);

            var result = await service.BringMyData();

            Assert.Null(result);
        }

        [Fact]
        public async Task BringMyFormulasAsync_UserWithoutFormula_ReturnsEmptyList()
        {
            var context = GetDbContext();

            var user = new User { Id = 3, Name = "UserNoFormula", Email = "noformula@user.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(user.Id);
            var service = new ProfileService(userManager, context, httpContextAccessor);

            var result = await service.BringMyFormulasAsync();

            Assert.Empty(result);
        }


        [Fact]
        public async Task BringMyProducts_UserHasProducts_ReturnsProductList()
        {
            var context = GetDbContext();

            var product1 = new Product { Id = 12, Name = "Producto 1", Description = "Desc 1", TipoProductoId = 1 };
            var product2 = new Product { Id = 9, Name = "Producto 2", Description = "Desc 2", TipoProductoId = 1 };
            context.AddRange(product1, product2);

            var user = new User
            {
                Id = 4,
                Name = "UserWithProducts",
                Email = "products@user.com",
                Products = new List<Product> { product1, product2 }
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(user.Id);
            var service = new ProfileService(userManager, context, httpContextAccessor);

            var result = await service.BringMyProducts();

            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Name == "Producto 1");
            Assert.Contains(result, p => p.Name == "Producto 2");
        }

        [Fact]
        public async Task UpdateMyData_UserExists_UpdatesUserData()
        {
            var context = GetDbContext();

            var user = new User
            {
                Id = 8,
                Name = "Old Name",
                Email = "oldemail@user.com"
            };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(user.Id);
            var service = new ProfileService(userManager, context, httpContextAccessor);

            var dto = new UserProfileUpdateDto
            {
                Name = "New Name",
                Email = "oldemail@user.com",
                EsProveedor = false
            };

            var result = await service.UpdateMyData(dto);

            Assert.NotNull(result);
            Assert.Equal("New Name", result.Name);

            var updatedUser = await context.Users.FindAsync(user.Id);
            Assert.Equal("New Name", updatedUser.Name);
        }

        [Fact]
        public async Task UpdateMyData_UserDoesNotExist_ReturnsNull()
        {
            var context = GetDbContext();

            var userManager = GetUserManagerMock();
            var httpContextAccessor = GetHttpContextAccessor(999);
            var service = new ProfileService(userManager, context, httpContextAccessor);

            var dto = new UserProfileUpdateDto
            {
                Name = "New Name",
                Email = "newemail@user.com",
                EsProveedor = false
            };

            var result = await service.UpdateMyData(dto);

            Assert.Null(result);
        }
    }
}
