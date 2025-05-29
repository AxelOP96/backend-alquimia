//using backendAlquimia.alquimia.Data;
using alquimia.Data.Data.Entities;
using backendAlquimia.alquimia.Services.Interfaces;
using backendAlquimia.Models;
using Microsoft.AspNetCore.Mvc;

namespace backendAlquimia.Controllers
{
    [Route("creator")]
    [ApiController]
    public class CreatorController : ControllerBase
    {
        private readonly INoteService _noteService;
        private readonly IProductService _productService;

        public CreatorController(INoteService notaService, IProductService productService)
        {
            _noteService = notaService;
            _productService = productService;
        }

        [HttpGet("base-notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetBaseNotes()
        {
            List<NotesGroupedByFamilyDTO> notes = await _noteService.GetBaseNotesGroupedByFamilyAsync();
            return Ok(notes);
        }

        [HttpGet("heart-notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetHeartNotes()
        {
            List<NotesGroupedByFamilyDTO> notas = await _noteService.GetHeartNotesGroupedByFamilyAsync();
            return Ok(notas);
        }

        [HttpGet("top-notes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetTopNotes()
        {
            List<NotesGroupedByFamilyDTO> notas = await _noteService.GetTopNotesGroupedByFamilyAsync();
            return Ok(notas);
        }

        [HttpPost("compatibilities")]
        public async Task<IActionResult> PostCompatibleNotes([FromBody] SelectedNotesDTO dto)
        {
            var compatibles = await _noteService.GetCompatibleNotesAsync(dto.ListaDeIdsSeleccionadas, dto.Sector);
            return Ok(compatibles);
        }

        [HttpGet("price-range")]
        public async Task<IActionResult> GetPriceRange([FromQuery] int noteId)
        {
            var PriceRange = await _productService.GetPriceRangeFromProductAsync(noteId);
            return Ok(PriceRange);
        }
    }
}
