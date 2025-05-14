using backendAlquimia.Data.Entities;
using backendAlquimia.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace backendAlquimia.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CreacionController : ControllerBase
    {
        private readonly INotaService _notaService;

        public CreacionController(INotaService notaService)
        {
            _notaService = notaService;
        }

        [HttpGet("notasDeSalida")]
        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasSalida()
        {
            var notas = await _notaService.ObtenerNotasDeSalidaAgrupadasPorFamiliaAsync();
            return Ok(notas);
        }

        [HttpGet("notasDeCorazon")]
        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasCorazon()
        {
            var notas = await _notaService.ObtenerNotasDeCorazonAgrupadasPorFamiliaAsync();
            return Ok(notas);
        }

        [HttpGet("notasDeFondo")]
        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasFondo()
        {
            var notas = await _notaService.ObtenerNotasDeFondoAgrupadasPorFamiliaAsync();
            return Ok(notas);
        }
    }
}
