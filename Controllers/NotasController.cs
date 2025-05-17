using Microsoft.AspNetCore.Mvc;
using backendAlquimia.Services;
using backendAlquimia.Data;
using backendAlquimia.Models;          // Para NotaDTO
using Microsoft.EntityFrameworkCore;    // Para Include

[ApiController]
[Route("api/[controller]")]
public class NotasController : ControllerBase
{
    private readonly ICompatibilityService _compatSvc;
    private readonly AlquimiaDbContext _context;

    public NotasController(AlquimiaDbContext context,
                           ICompatibilityService compatSvc)
    {
        _context = context;
        _compatSvc = compatSvc;
    }

    // GET /api/notas           (sin cambios)
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_context.Notas.Include(n => n.FamiliaOlfativa).ToList());

    /* ---------- NUEVO: acepta GET o POST --------------- */

    // GET  /api/notas/compatibles?ids=1&ids=4&ids=7
    [HttpGet("compatibles")]
    public IActionResult CompatiblesGet([FromQuery(Name = "ids")] int[] ids) =>
        Compatibles(ids);

    // POST /api/notas/compatibles   (body: [1,4,7])
    [HttpPost("compatibles")]
    public IActionResult CompatiblesPost([FromBody] int[] ids) =>
        Compatibles(ids);

    /* ------------- lógica común ------------------------ */
    private IActionResult Compatibles(int[] ids)
    {
        ids ??= Array.Empty<int>();

        // ‼️ Si no hay seleccionadas sólo devuelvo notas “candidatas” (self-score >= 60)
        var seleccionadas = _context.Notas
                            .Include(n => n.FamiliaOlfativa)
                            .Where(n => ids.Contains(n.Id))
                            .ToList();

        var todas = _context.Notas
                     .Include(n => n.FamiliaOlfativa)
                     .ToList();

        var sugeridas = _compatSvc
                        .GetCompatibleNotes(seleccionadas, todas)
                        .Select(n => new NotaDTO   // ← ya existe en Models
                        {
                            Id = n.Id,
                            Nombre = n.Nombre,
                            Familia = n.FamiliaOlfativa?.Nombre,
                            Sector = n.Sector?.Nombre,
                            Descripcion = n.Descripcion
                        });



        return Ok(sugeridas);
    }
}
