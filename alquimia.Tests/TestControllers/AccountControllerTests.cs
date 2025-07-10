using alquimia.Api.Controllers;
using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<ILogger<AccountController>> _loggerMock;
        private readonly Mock<IJwtService> _jwtServiceMock;
        private readonly Mock<IEmailService> _emailServiceMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly Mock<IEmailTemplateService> _emailTemplateMock;
        private readonly AlquimiaDbContext _context;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            var userStoreMock = new Mock<IUserStore<User>>();
            _userManagerMock = new Mock<UserManager<User>>(userStoreMock.Object, null, null, null, null, null, null, null, null);

            var contextAccessorMock = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
            var userPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object, contextAccessorMock.Object, userPrincipalFactoryMock.Object, null, null, null, null);

            _loggerMock = new Mock<ILogger<AccountController>>();
            _jwtServiceMock = new Mock<IJwtService>();
            _emailServiceMock = new Mock<IEmailService>();
            _configMock = new Mock<IConfiguration>();
            _emailTemplateMock = new Mock<IEmailTemplateService>();

            _controller = new AccountController(_userManagerMock.Object, _signInManagerMock.Object, _loggerMock.Object, _jwtServiceMock.Object, _emailServiceMock.Object, _configMock.Object, _emailTemplateMock.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            var dto = new RegisterDTO { Email = "existente@alquimia.com", Password = "Password123!", Name = "Existente", Rol = "Creador" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(new User());

            var result = await _controller.Register(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ya está registrado", badRequest.Value.ToString());
        }


        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            var dto = new LoginDTO { Email = "wrong@alquimia.com", Password = "WrongPass" };
            var user = new User { Id = 3, Email = dto.Email, UserName = "wronguser" };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(user);
            _signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, dto.Password, false)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            var result = await _controller.Login(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Credenciales inválidas", unauthorized.Value.ToString());
        }

        [Fact]
        public async Task RegisterProvider_ShouldReturnBadRequest_WhenEmailExists()
        {
            var dto = new RegisterProviderDTO
            {
                Email = "existente@alquimia.com",
                Password = "Password123!",
                Name = "Proveedor Existente"
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(new User());

            var result = await _controller.RegisterProvider(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("ya está registrado", badRequest.Value.ToString());
        }



        [Fact]
        public async Task RegisterProvider_ShouldSetEsProveedorTrue_OnUserCreation()
        {
            var dto = new RegisterProviderDTO
            {
                Email = "nuevo@proveedor.com",
                Password = "Password123!",
                Name = "Nuevo",
                Empresa = "Fragancias SRL",
                Cuil = "20-11111111-1",
                Rubro = "Cosmética",
                OtroProducto = "Velas"
            };

            User? capturedUser = null;

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null);
            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), dto.Password))
                .Callback<User, string>((u, _) => capturedUser = u)
                .ReturnsAsync(IdentityResult.Success);

            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync(() => capturedUser!);
            _userManagerMock.Setup(x => x.IsInRoleAsync(It.IsAny<User>(), "Creador")).ReturnsAsync(false);
            _userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), "Creador")).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<User>())).ReturnsAsync(new List<string> { "Creador" });
            _jwtServiceMock.Setup(x => x.GenerateToken(It.IsAny<User>(), It.IsAny<IList<string>>())).Returns("token-generado");

            var result = await _controller.RegisterProvider(dto);

            Assert.True(capturedUser != null && capturedUser.EsProveedor);
        }
        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenEmailOrPasswordIsEmpty()
        {
            var dto = new LoginDTO { Email = "", Password = "" };

            var result = await _controller.Login(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("Email y contraseña son obligatorios", badRequest.Value.ToString());
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            var dto = new LoginDTO { Email = "noexiste@alquimia.com", Password = "Password123!" };
            _userManagerMock.Setup(x => x.FindByEmailAsync(dto.Email)).ReturnsAsync((User)null);

            var result = await _controller.Login(dto);

            var unauthorized = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Contains("Usuario no encontrado", unauthorized.Value.ToString());
        }

        [Fact]
        public async Task ObtenerPerfil_ShouldReturnCorrectUserData()
        {
            var user = new User { Email = "usuario@alquimia.com", Name = "Usuario" };
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Email, user.Email) }, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            _userManagerMock.Setup(x => x.FindByEmailAsync(user.Email)).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(new List<string> { "Creador" });

            var result = await _controller.ObtenerPerfil();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("usuario@alquimia.com", okResult.Value.ToString());
            Assert.Contains("Usuario", okResult.Value.ToString());
            Assert.Contains("Creador", okResult.Value.ToString());
        }

        [Fact]
        public void State_ShouldReturnAuthStatus()
        {
            var identity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, "TestUser") }, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };

            var result = _controller.State();

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Contains("autenticado", okResult.Value.ToString());
            Assert.Contains("TestUser", okResult.Value.ToString());
        }

    }
}
