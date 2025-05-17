using Microsoft.AspNetCore.Mvc;
using backendAlquimia.Services;
using backendAlquimia.Data; // <-- necesario para ApplicationDbContext
using backendAlquimia.Data.Entities; // <-- necesario para Nota
using Microsoft.EntityFrameworkCore; // <-- necesario si usás Include
using System.Linq;

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

    // GET /api/notas
    [HttpGet]
    public IActionResult GetAll() =>
        Ok(_context.Notas.ToList());

    // POST /api/notas/compatibles
    [HttpPost("compatibles")]
    public IActionResult GetCompatibles([FromBody] int[] seleccionadas)
    {
        var elegidas = _context.Notas
                        .Where(n => seleccionadas.Contains(n.Id))
                        .ToList();
        var todas = _context.Notas.ToList();
        var sugeridas = _compatSvc
                        .GetCompatibleNotes(elegidas, todas)
                        .ToList();
        return Ok(sugeridas);
    }
}
