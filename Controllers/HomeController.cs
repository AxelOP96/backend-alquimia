using System.Diagnostics;
using System.Security.Claims;
using backendAlquimia.Models;
using Microsoft.AspNetCore.Mvc;

namespace backendAlquimia.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        /*
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }*/
        [HttpGet("/home/")]
        public IActionResult ObtenerUsuarioInfo()
        {
            var identity = HttpContext.User.Identity;

            if (identity != null && identity.IsAuthenticated)
            {
                var nombre = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var email = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

                return Ok(new
                {
                    autenticado = true,
                    nombre,
                    email
                });
            }

            return Ok(new
            {
                autenticado = false,
                mensaje = "Usuario no autenticado"
            });
        }

    }
}
