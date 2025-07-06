using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Mvc;

namespace Alquimia.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MercadoLibreController : ControllerBase
    {
        private readonly IMercadoLibreService _meliService;

        public MercadoLibreController(IMercadoLibreService meliService)
        {
            _meliService = meliService;
        }

        /// <summary>
        /// Endpoint llamado por Mercado Libre cuando se realiza una venta.
        /// Body: { variantId, quantity }
        /// </summary>
        [HttpPost("order-notification")]
        public async Task<IActionResult> OrderNotification([FromBody] MercadoLibreOrderDTO dto)
        {
            try
            {
                await _meliService.ProcessOrderAsync(dto);
                return Ok();
            }
            catch (KeyNotFoundException knf)
            {
                return NotFound(knf.Message);
            }
            catch (InvalidOperationException inv)
            {
                return BadRequest(inv.Message);
            }
        }
    }
}
