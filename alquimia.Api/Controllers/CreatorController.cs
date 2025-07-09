using alquimia.Api.Helpers;
using alquimia.Services;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alquimia.Api.Controllers
{
    [Authorize(Roles = "Creador")]
    [Route("creator")]
    [ApiController]
    public class CreatorController : ControllerBase
    {
        private readonly INoteService _notaService;
        private readonly IFormulaService _formulaService;
        private readonly IOlfactoryFamilyService _olfactoryFamilyService;
        private readonly IDesignLabelService _designLabelService;
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _emailTemplate;

        public CreatorController(INoteService notaService, IFormulaService formulaService, IOlfactoryFamilyService olfactoryFamilyService,
            IDesignLabelService designLabelService, IEmailService emailService, IEmailTemplateService emailTemplate)
        {
            _notaService = notaService;
            _formulaService = formulaService;
            _olfactoryFamilyService = olfactoryFamilyService;
            _designLabelService = designLabelService;
            _emailService = emailService;
            _emailTemplate = emailTemplate;
        }

        [HttpGet("create")]
        public IActionResult Create()
        {
            return Ok("Ruta activa");
        }

        [HttpGet("start")]
        public IActionResult Start()
        {
            return Ok("Ruta activa");
        }

        [HttpGet("{sector}-notes")]
        public async Task<ActionResult<IEnumerable<NotesGroupedByFamilyDTO>>> GetNotesBySector(string sector)
        {
            if (!SectorMapper.TryMapToSpanish(sector, out var mappedSector))
                throw new ArgumentNullException();

            var notes = await _notaService.GetNotesGroupedByFamilyAsync(mappedSector);
            return Ok(notes);
        }

        [HttpPost("compatibilities")]
        public async Task<IActionResult> PostCompatibleNotes([FromBody] SelectedNotesDTO dto)
        {
            var compatibles = await _notaService.GetCompatibleNotesAsync(dto.ListaDeIdsSeleccionadas, dto.Sector);
            return Ok(compatibles);
        }

        [HttpPost("envase-pdf")]
        public IActionResult DescargarPdf([FromBody] DesignDTO dto)
        {
            var pdfBytes = DesignLabelService.CreatePdfDesign(dto);
            return File(pdfBytes, "application/pdf", "myDesign.pdf");
        }


        [HttpGet("intensities")]
        public async Task<ActionResult<IEnumerable<IntensityDTO>>> GetIntensities()
        {
            var intensities = await _formulaService.GetIntensitiesAsync();
            return Ok(intensities);
        }

        [HttpPost("save-formula")]
        public async Task<IActionResult> SaveFormula([FromBody] POSTFormulaDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var formulaId = await _formulaService.SaveAsync(dto);
            return Ok(new { formulaId });
        }

        [HttpGet("get-formula/{id}")]
        public async Task<IActionResult> GetFormulaById(int id)
        {
            var formula = await _formulaService.GetFormulaByIdToDTOAsync(id);
            return Ok(formula);
        }

        [HttpGet("note-info/{id}")]
        public async Task<IActionResult> GetNoteInfo(int id)
        {
            var note = await _notaService.GetNoteInfoAsync(id);
            return Ok(note);
        }

        [HttpGet("family-info/{id}")]
        public async Task<IActionResult> GetOlfactoryFamilyInfo(int id)
        {
            var family = await _olfactoryFamilyService.GetOlfactoryFamilyInfoAsync(id);
            return Ok(family);
        }

        [HttpPost("save-design")]
        public async Task<IActionResult> SaveDesign([FromBody] DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var designId = await _designLabelService.SaveDesignAsync(dto);
                return Ok(new { designId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error al guardar el diseño", details = ex.Message });
            }
        }

        [HttpPatch("formula/{id}/titulo")]
        public async Task<IActionResult> UpdateTitle(int id, [FromBody] TitleDTO dto)
        {
            var found = await _formulaService.GetFormulaAsync(id);
            await _formulaService.UpdateTitleAsync(found, dto.Title);
            return NoContent();
        }

        [HttpPost("send-pdf")]
        public async Task<IActionResult> SendPDFToProvider(
    [FromForm] SendPDFToProvider dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email))
                throw new KeyNotFoundException("El email es obligatorio.");

            if (dto.File == null || dto.File.Length == 0)
                throw new ArgumentNullException("No se recibió el archivo PDF.");

            byte[] attachmentBytes;

            using (var memoryStream = new MemoryStream())
            {
                await dto.File.CopyToAsync(memoryStream);
                attachmentBytes = memoryStream.ToArray();
            }

            var message = _emailTemplate.GetDesignPDFProviderEmail(dto.ProviderName, dto.CreatorName);

            var result = await _emailService.SendEmailWithAttachmentAsync(
                dto.Email,
                "PDF del diseño de etiqueta",
                message,
                attachmentBytes,
                dto.File.FileName
            );

            if (!result)
                return StatusCode(500, "Ocurrió un error al enviar el correo.");

            return Ok("Correo enviado correctamente.");
        }

    }
}
