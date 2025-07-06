using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using User = alquimia.Data.Entities.User;
//
namespace alquimia.Api.Controllers
{
    [ApiController]
    [Route("/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _config;
        private readonly IEmailTemplateService _emailTemplate;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager,
            ILogger<AccountController> logger, IJwtService jwtService, IEmailService emailService, IConfiguration config, IEmailTemplateService emailTemplate)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtService = jwtService;
            _emailService = emailService;
            _config = config;
            _emailTemplate = emailTemplate;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO dto)
        {
            _logger.LogInformation("Intentando registrar usuario con email: {Email}", dto.Email);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioExistente != null)
                return BadRequest(new { mensaje = "El email ya está registrado." });

            var nuevoUsuario = new User
            {
                UserName = GenerateUserNameSeguro(dto.Email),
                Email = dto.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                Name = dto.Name?.Trim()
            };

            var result = await _userManager.CreateAsync(nuevoUsuario, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Error al crear el usuario: {Errores}", result.Errors);
                return BadRequest(result.Errors);
            }

            var usuarioPersistido = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioPersistido == null)
                return StatusCode(500, new { mensaje = "No se pudo recuperar el usuario recién creado." });

            if (!await _userManager.IsInRoleAsync(usuarioPersistido, dto.Rol))
            {
                var roleResult = await _userManager.AddToRoleAsync(usuarioPersistido, dto.Rol);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Error al asignar el rol: {Errores}", roleResult.Errors);
                    return BadRequest(new { mensaje = "Error al asignar el rol." });
                }
            }

            var roles = await _userManager.GetRolesAsync(usuarioPersistido);
            var token = _jwtService.GenerateToken(usuarioPersistido, roles);

            await _signInManager.SignInAsync(usuarioPersistido, isPersistent: false);
            _logger.LogInformation("Usuario registrado exitosamente: {Email}", dto.Email);
            return Ok(new { mensaje = "Usuario registrado correctamente.", token });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            _logger.LogInformation("Intentando login para el email: {Email}", dto.Email);

            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { mensaje = "Email y contraseña son obligatorios." });

            var usuario = await _userManager.FindByEmailAsync(dto.Email);

            if (usuario == null)
            {
                _logger.LogWarning("Intento de login con email no registrado: {Email}", dto.Email);
                return Unauthorized(new { mensaje = "Usuario no encontrado." });
            }

            if (string.IsNullOrWhiteSpace(usuario.UserName) || usuario.Id == 0)
                return StatusCode(500, new { mensaje = "El usuario tiene datos incompletos (UserName o Id)." });

            var result = await _signInManager.CheckPasswordSignInAsync(usuario, dto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new { mensaje = "Credenciales inválidas." });

            var roles = await _userManager.GetRolesAsync(usuario);
            var token = _jwtService.GenerateToken(usuario, roles);
            await _signInManager.SignInAsync(usuario, isPersistent: false);
            _logger.LogInformation("Login exitoso para {Email}", dto.Email);
            return Ok(new { mensaje = "Login exitoso ✅", token });
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle([FromQuery] bool debug = false)
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Account", null, Request.Scheme);
            _logger.LogInformation("🔁 redirect_uri usado para Google OAuth: {RedirectUrl}", redirectUrl);
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            if (debug)
            {
                return Ok(new
                {
                    redirectUrl,
                    callback = redirectUrl
                });
            }
            return Challenge(properties, "Google");
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLoginCallback([FromQuery] bool debug = false)
        {
            _logger.LogInformation("Callback de login con Google recibido");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Fallo al obtener la información de login externo.");
                if (debug)
                    return BadRequest(new { mensaje = "No se pudo obtener la información externa." });
                return Redirect("http://localhost:3000/Login?error=callback");
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            var frontendRedirect = _config["OAuth:Url"];
            if (string.IsNullOrWhiteSpace(frontendRedirect) ||
                frontendRedirect.Contains("/account/signin-google", StringComparison.OrdinalIgnoreCase))
            {
                var baseUrl = _config["AppSettings:FrontendBaseUrl"] ?? "https://frontend-alquimia.vercel.app/";
                frontendRedirect = baseUrl.TrimEnd('/') + "/Login/RedirectGoogle";
            }

            if (result.Succeeded)
            {
                var existingUser = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                var rolesExisting = await _userManager.GetRolesAsync(existingUser);
                var tokenExisting = _jwtService.GenerateToken(existingUser, rolesExisting);
                await _signInManager.SignInAsync(existingUser, isPersistent: false);
                if (debug)
                {
                    return Ok(new
                    {
                        token = tokenExisting,
                        existingUser = true,
                        email = info.Principal.FindFirstValue(ClaimTypes.Email),
                        name = info.Principal.FindFirstValue(ClaimTypes.Name)
                    });
                }
                return Redirect(frontendRedirect);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var newUser = new User
            {
                Email = email,
                UserName = GenerateUserNameSeguro(email),
                Name = name,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
                return Redirect("http://localhost:3000/Login?error=creation");

            await _userManager.AddLoginAsync(newUser, info);
            var roles = await _userManager.GetRolesAsync(newUser);
            var token = _jwtService.GenerateToken(newUser, roles);
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            _logger.LogInformation("Google login info recibida para: {Email}", email);

            if (debug)
            {
                return Ok(new
                {
                    token,
                    newUser = true,
                    email,
                    name
                });
            }
            return Redirect(frontendRedirect);
        }

        [HttpGet("debug/google-config")]
        public IActionResult GoogleConfigDebug()
        {
            var loginEndpoint = Url.Action("LoginWithGoogle", "Account", null, Request.Scheme);
            var callback = Url.Action("GoogleLoginCallback", "Account", null, Request.Scheme);
            var frontendRedirect = _config["OAuth:Url"];
            if (string.IsNullOrWhiteSpace(frontendRedirect) ||
                frontendRedirect.Contains("/account/signin-google", StringComparison.OrdinalIgnoreCase))
            {
                var baseUrl = _config["AppSettings:FrontendBaseUrl"] ?? "https://frontend-alquimia.vercel.app/";
                frontendRedirect = baseUrl.TrimEnd('/') + "/Login/RedirectGoogle";
            }
            return Ok(new
            {
                clientIdDefined = !string.IsNullOrWhiteSpace(_config["OAuth:ClientID"]),
                loginEndpoint,
                callback,
                frontendRedirect
            });
        }

        [HttpPost("register-provider")]
        public async Task<IActionResult> RegisterProvider([FromBody] RegisterProviderDTO dto)
        {
            _logger.LogInformation("Intentando registrar proveedor con email: {Email}", dto.Email);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioExistente != null)
                return BadRequest(new { mensaje = "El email ya está registrado." });

            var nuevoUsuario = new User
            {
                UserName = GenerateUserNameSeguro(dto.Email),
                Email = dto.Email,
                Name = dto.Name?.Trim(),
                EsProveedor = true,
                Empresa = dto.Empresa,
                Cuil = dto.Cuil,
                Rubro = dto.Rubro,
                OtroProducto = string.Join(",", dto.OtroProducto),
                TarjetaNumero = dto.TarjetaNumero,
                TarjetaVencimiento = dto.TarjetaVencimiento,
                TarjetaCVC = dto.TarjetaCVC
            };

            var result = await _userManager.CreateAsync(nuevoUsuario, dto.Password);
            if (!result.Succeeded)
            {
                _logger.LogError("Error al crear el usuario proveedor: {Errores}", result.Errors);
                return BadRequest(result.Errors);
            }

            var usuarioPersistido = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioPersistido == null)
                return StatusCode(500, new { mensaje = "No se pudo recuperar el proveedor recién creado." });

            var rolInicial = "Creador";
            if (!await _userManager.IsInRoleAsync(usuarioPersistido, rolInicial))
            {
                var roleResult = await _userManager.AddToRoleAsync(usuarioPersistido, rolInicial);
                if (!roleResult.Succeeded)
                {
                    _logger.LogError("Error al asignar rol inicial: {Errores}", roleResult.Errors);
                    return BadRequest(new { mensaje = "Error al asignar el rol inicial." });
                }
            }

            var roles = await _userManager.GetRolesAsync(usuarioPersistido);
            var token = _jwtService.GenerateToken(usuarioPersistido, roles);

            await _signInManager.SignInAsync(usuarioPersistido, isPersistent: false);
            _logger.LogInformation("Proveedor registrado exitosamente como Creador: {Email}", dto.Email);
            var mensajeBienvenida = _emailTemplate.GetWelcomeEmail(dto.Name);
            await _emailService.SendEmailAsync(dto.Email, "Bienvenido a Alquimia - Cuenta en revisión", mensajeBienvenida);
            return Ok(new { mensaje = "Proveedor registrado correctamente como creador en espera de aprobación.", token });
        }

        [HttpGet("auth/status")]
        public IActionResult State()
        {
            var usuario = User.Identity;
            return Ok(new
            {
                autenticado = usuario?.IsAuthenticated ?? false,
                nombre = usuario?.Name
            });
        }

        [Authorize]
        [HttpGet("perfil")]
        public async Task<IActionResult> ObtenerPerfil()
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _userManager.FindByEmailAsync(userEmail);
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new
            {
                id = user.Id,
                nombre = user.Name,
                email = user.Email,
                rol = roles.FirstOrDefault()
            });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var frontendBaseUrl = _config["AppSettings:FrontendBaseUrl"];
            var callbackUrl = $"{frontendBaseUrl}restablecer-contrasenia?email={model.Email}&token={encodedToken}";
            var message = _emailTemplate.GetPasswordResetEmail(user.Name, callbackUrl);
            await _emailService.SendEmailAsync(model.Email, "Recuperar contraseña - Alquimia", message);
            return Ok("Se envió un enlace para restablecer la contraseña.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                throw new KeyNotFoundException("Usuario no encontrado");

            var result = await _userManager.ResetPasswordAsync(user, WebUtility.UrlDecode(model.Token), model.NewPassword);
            if (!result.Succeeded)
                throw new ArgumentException("Ocurrió un error:" + string.Join(", ", result.Errors.Select(e => e.Description)));

            return Ok("Contraseña restablecida correctamente.");
        }

        private string GenerateUserNameSeguro(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
                return Guid.NewGuid().ToString("N").Substring(0, 8);

            var nombre = email.Split('@')[0];

            nombre = new string(nombre.Where(char.IsLetterOrDigit).ToArray());

            return string.IsNullOrWhiteSpace(nombre)
                ? Guid.NewGuid().ToString("N").Substring(0, 8)
                : nombre;
        }
    }
}
