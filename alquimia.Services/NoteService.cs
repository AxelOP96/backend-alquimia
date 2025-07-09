using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using Note = alquimia.Data.Entities.Note;

namespace alquimia.Services
{
    public class NoteService : INoteService
    {
        private readonly AlquimiaDbContext _context;

        public NoteService(AlquimiaDbContext context)
        {
            _context = context;
        }

        public Task<List<NotesGroupedByFamilyDTO>> GetNotesGroupedByFamilyAsync(string sectorInSpanish)
        {
            return _context.Notes
                .Include(n => n.OlfactoryPyramid)
                .Include(n => n.OlfactoryFamily)
                .Where(n => n.OlfactoryPyramid.Sector == sectorInSpanish)
                .GroupBy(n => new { n.OlfactoryFamily.Id, n.OlfactoryFamily.Nombre })
                .Select(grupo => new NotesGroupedByFamilyDTO
                {
                    Family = grupo.Key.Nombre,
                    FamilyId = grupo.Key.Id,
                    Notes = grupo.Select(n => new NoteDTO
                    {
                        Id = n.Id,
                        Name = n.Name,
                        Family = n.OlfactoryFamily.Nombre,
                        Sector = n.OlfactoryPyramid.Sector,
                        Description = n.Description,
                        Duration = n.OlfactoryPyramid.Duracion,
                        Image = n.Image
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<List<NotesGroupedByFamilyDTO>> GetCompatibleNotesAsync(List<int> seleccionadasIds, string sector)
        {
            var seleccionadas = await _context.Notes
                .Where(n => seleccionadasIds.Contains(n.Id) && n.OlfactoryPyramid.Sector == sector)
                .Include(n => n.OlfactoryFamily)
                .Include(n => n.OlfactoryPyramid)
                .ToListAsync();
            var todasLasNotasDelSector = await _context.Notes
                .Where(n => n.OlfactoryPyramid.Sector == sector)
                .Include(n => n.OlfactoryFamily)
                .Include(n => n.OlfactoryPyramid)
                .ToListAsync();
            var incompatibilidades = await _context.IncompatibleNotes.ToListAsync();
            var compatibilidades = await _context.FamilyCompatibilities.ToListAsync();
            var compatiblesConCompatibilidad = new List<(Note Nota, int MinCompatibilidad)>();
            foreach (var candidata in todasLasNotasDelSector)
            {
                if (seleccionadas.Any(n => n.Id == candidata.Id))
                    continue;
                bool esCompatible = true;
                int minCompatibilidad = int.MaxValue;
                foreach (var seleccionada in seleccionadas)
                {
                    if (incompatibilidades.Any(i =>
                        (i.NotaId == seleccionada.Id && i.NotaIncompatibleId == candidata.Id) ||
                        (i.NotaId == candidata.Id && i.NotaIncompatibleId == seleccionada.Id)))
                    {
                        esCompatible = false;
                        break;
                    }
                    int f1 = Math.Min(seleccionada.OlfactoryFamilyId, candidata.OlfactoryFamilyId);
                    int f2 = Math.Max(seleccionada.OlfactoryFamilyId, candidata.OlfactoryFamilyId);
                    var compat = compatibilidades.FirstOrDefault(c =>
                        c.FamiliaMenor == f1 && c.FamiliaMayor == f2);
                    if (compat == null || compat.GradoDeCompatibilidad < 70)
                    {
                        esCompatible = false;
                        break;
                    }
                    minCompatibilidad = Math.Min(minCompatibilidad, compat.GradoDeCompatibilidad);
                }
                if (esCompatible) compatiblesConCompatibilidad.Add((candidata, minCompatibilidad));
            }
            var resultado = compatiblesConCompatibilidad
                .OrderByDescending(c => c.MinCompatibilidad)
                .GroupBy(c => c.Nota.OlfactoryFamily.Nombre)
                .Select(g => new NotesGroupedByFamilyDTO
                {
                    Family = g.Key,
                    Notes = g.Select(c => new NoteDTO
                    {
                        Id = c.Nota.Id,
                        Name = c.Nota.Name,
                        Description = c.Nota.Description,
                        Family = c.Nota.OlfactoryFamily.Nombre,
                        Sector = c.Nota.OlfactoryPyramid.Sector,
                        Duration = c.Nota.OlfactoryPyramid.Duracion,
                        Image = c.Nota.Image
                    }).ToList()
                })
                .ToList();
            return resultado;
        }

        public async Task<NoteDTO> GetNoteInfoAsync(int id) // returns a note, its family and olfactory pyramid
        {
            var found = await _context.Notes
                .Include(n => n.OlfactoryPyramid)
                .Include(n => n.OlfactoryFamily)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (found == null)
            {
                throw new KeyNotFoundException();
            }

            return NoteToDTO(found);
        }
        public async Task<List<string>> GetNoteNamesBySectorAsync(string sector)
        {
            return await _context.Notes
                .Where(n => n.OlfactoryPyramid.Sector == sector)
                .Select(n => n.Name)
                .Distinct()
                .Take(10)
                .ToListAsync();
        }

        private NoteDTO NoteToDTO(Note found)
        {
            return new NoteDTO
            {
                Id = found.Id,
                Name = found.Name,
                Family = found.OlfactoryFamily.Nombre,
                Sector = found.OlfactoryPyramid.Sector,
                Description = found.Description,
                Duration = found.OlfactoryPyramid.Duracion,
                Image = found.Image
            };
        }
    }
}
