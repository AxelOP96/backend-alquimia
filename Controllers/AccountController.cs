﻿using System.Security.Claims;
//using backendAlquimia.alquimia.Data;
using backendAlquimia.Models;
using backendAlquimia.alquimia.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using alquimia.Data.Data.Entities;

namespace backendAlquimia.Controllers
{
    [ApiController]
    [Route("cuenta")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IJwtService _jwtService;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, ILogger<AccountController> logger, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _jwtService = jwtService;
        }

        [HttpPost("registrar-json")]
        public async Task<IActionResult> RegistrarJson([FromBody] RegisterDTO dto)
        {
            _logger.LogInformation("Intentando registrar usuario con email: {Email}", dto.Email);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var usuarioExistente = await _userManager.FindByEmailAsync(dto.Email);
            if (usuarioExistente != null)
                return BadRequest(new { mensaje = "El email ya está registrado." });

            var nuevoUsuario = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                Name = dto.Name?.Trim()
            };

            var result = await _userManager.CreateAsync(nuevoUsuario, dto.Password);
            var roleExists = await _userManager.IsInRoleAsync(nuevoUsuario, dto.Rol);
            if (!await _userManager.IsInRoleAsync(nuevoUsuario, dto.Rol))
            {
                var roleResult = await _userManager.AddToRoleAsync(nuevoUsuario, dto.Rol);
                if (!roleResult.Succeeded)
                    return BadRequest(new { mensaje = "Error al asignar el rol." });
            }
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var roles = await _userManager.GetRolesAsync(nuevoUsuario);
            var token = _jwtService.GenerateToken(nuevoUsuario, roles);
            // Autenticamos automáticamente al usuario después del registro
            await _signInManager.SignInAsync(nuevoUsuario, isPersistent: false);
            _logger.LogInformation("Usuario registrado exitosamente: {Email}", dto.Email);
            return Ok(new { mensaje = "Usuario registrado correctamente.", token });
        }

        [HttpPost("login-json")]
        public async Task<IActionResult> LoginJson([FromBody] LoginDTO dto)
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
        public IActionResult LoginWithGoogle()
        {
            var redirectUrl = Url.Action("GoogleLoginCallback", "Cuenta");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [HttpGet("signin-google")]
        public async Task<IActionResult> GoogleLoginCallback()
        {
            _logger.LogInformation("Callback de login con Google recibido");
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                _logger.LogError("Fallo al obtener la información de login externo.");
                return Redirect("http://localhost:3000/Login?error=callback");
            }


            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
            {
                return Redirect("http://localhost:3000/login/redirectgoogle");
            }

            // Crear el usuario si no existe
            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(ClaimTypes.Name);

            var newUser = new User
            {
                Email = email,
                UserName = email,
                Name = name
            };

            var createResult = await _userManager.CreateAsync(newUser);
            if (!createResult.Succeeded)
                return Redirect("http://localhost:3000/Login?error=creation");
            var roles = await _userManager.GetRolesAsync(newUser);
            var token = _jwtService.GenerateToken(newUser, roles);
            await _userManager.AddLoginAsync(newUser, info);
            await _signInManager.SignInAsync(newUser, isPersistent: false);
            _logger.LogInformation("Google login info recibida para: {Email}", info.Principal.FindFirstValue(ClaimTypes.Email));
            return Redirect("http://localhost:3000/Login/RedirectGoogle");
        }

        [HttpGet("auth/status")]
        public IActionResult Estado()
        {
            var usuario = User.Identity;
            return Ok(new
            {
                autenticado = usuario?.IsAuthenticated ?? false,
                nombre = usuario?.Name
            });
        }
    }
}
