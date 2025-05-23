//using backendAlquimia.Data.Entities;
//using backendAlquimia.Models;
//using backendAlquimia.Services.Interfaces;
//using Microsoft.AspNetCore.Mvc;

//namespace backendAlquimia.Controllers
//{
//    [Route("[controller]")]
//    [ApiController]
//    public class CreacionController : ControllerBase
//    {
//        private readonly INotaService _notaService;
//        private readonly IFormulaService _formulaService;

//        public CreacionController(INotaService notaService, IFormulaService formulaService)
//        {
//            _notaService = notaService;
//            _formulaService = formulaService;
//        }
//        // tiene que ser la primera solicitud.
//        [HttpGet("notasDeFondo")]
//        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasFondo()
//        {
//            List<NotasPorFamiliaDTO> notas = await _notaService.ObtenerNotasDeFondoAgrupadasPorFamiliaAsync();
//            return Ok(notas);
//        }

//        [HttpGet("notasDeCorazon")]
//        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasCorazon()
//        {
//            List<NotasPorFamiliaDTO> notas = await _notaService.ObtenerNotasDeCorazonAgrupadasPorFamiliaAsync();
//            return Ok(notas);
//        }

//        [HttpGet("notasDeSalida")]
//        public async Task<ActionResult<IEnumerable<Nota>>> GetNotasSalida()
//        {
//            List<NotasPorFamiliaDTO> notas = await _notaService.ObtenerNotasDeSalidaAgrupadasPorFamiliaAsync();
//            return Ok(notas);
//        }

//        [HttpGet("grado")]
//        public async Task<IActionResult> ObtenerGradoCompatibilidad(int notaAId, int notaBId)
//        {
//            var grado = await _notaService.CalcularCompatibilidadAsync(notaAId, notaBId);
//            return Ok(new { grado });
//        }

//        [HttpPost("sugerencias")]
//        public async Task<IActionResult> ObtenerNotasCompatiblesAsync([FromBody] NotasSeleccionadasDTO dto)
//        {
//            var compatibles = await _notaService.ObtenerNotasCompatiblesAsync(dto.ListaDeIdsSeleccionadas, dto.Sector);
//            return Ok(compatibles);
//        }

//        [HttpGet("intensidad")]
//        public async Task<IActionResult> GetIntensidad()
//        {
//            List<IntensidadDTO> intensidad = await _formulaService.ObtenerIntensidadAsync();
//            return Ok(intensidad);
//        }

//        [HttpPost("formular")]
//        public async Task<IActionResult> FinalizarCreacion([FromBody] POSTFormulaDTO dto)
//        {
//            GETFormulaDTO formulaGuardada = await _formulaService.guardar(dto);
//            return CreatedAtAction(
//                nameof(ObtenerFormulaPorId),
//                new { id = formulaGuardada.Id },
//                formulaGuardada
//                );
//        }

//        [HttpGet("formulas/{id}")]
//        public async Task<IActionResult> ObtenerFormulaPorId(int id)
//        {
//            var formula = await _formulaService.ObtenerPorId(id);
//            if (formula == null) return NotFound();
//            return Ok(formula);
//        }



//        // user envia nota
//        // la recibo
//        // proceso esa info para mostrarle las compatibles
//        // actualizo y le devuelvo las compatibles
//        // asi hasta llegar a 4 o hasta confirmar la seleccion

//        // necesito que el front me haga post cada vez que el user arrastra la nota al frasco. no se como, cookie? session? local storage? 
//        // necesito que el front me haga post cada vez que el user arrastra la nota al frasco. no se como, cookie? session? local storage?. UNa vez que el usuario ya selecciono todas las notas, hace click en CONFIRMAR Y AHI SE HACE esa solicitud /creacion/confirmar.

//    }
//}
