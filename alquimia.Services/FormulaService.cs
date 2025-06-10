using alquimia.Data.Entities;
using alquimia.Services.Extensions;
using alquimia.Services.Helpers;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Formula = alquimia.Data.Entities.Formula;
using FormulaNote = alquimia.Data.Entities.FormulaNote;
using Note = alquimia.Data.Entities.Note;

namespace alquimia.Services
{
    public class FormulaService : IFormulaService
    {
        private readonly AlquimiaDbContext _context;
        public FormulaService(AlquimiaDbContext context)
        {
            _context = context;
        }

        public async Task<List<IntensityDTO>> GetIntensitiesAsync()
        {
            return await _context.Intensities
                .Select
                (x => new IntensityDTO
                {
                    Id = x.Id,
                    Name = x.Nombre,
                    Description = x.Description,
                    Category = x.Category
                }).ToListAsync();
        }

        public async Task<int> SaveAsync(POSTFormulaDTO dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var top = new FormulaNote
                {
                    NotaId1 = dto.TopNotes.Note1.Id,
                    NotaId2 = dto.TopNotes.Note2?.Id,
                    NotaId3 = dto.TopNotes.Note3?.Id,
                    NotaId4 = dto.TopNotes.Note4?.Id
                };
                _context.FormulaNotes.Add(top);
                await _context.SaveChangesAsync();
                Console.WriteLine(top.FormulaNotaId);

                var heart = new FormulaNote
                {
                    NotaId1 = dto.HeartNotes.Note1.Id,
                    NotaId2 = dto.HeartNotes.Note2?.Id,
                    NotaId3 = dto.HeartNotes.Note3?.Id,
                    NotaId4 = dto.HeartNotes.Note4?.Id
                };
                _context.FormulaNotes.Add(heart);
                await _context.SaveChangesAsync();
                Console.WriteLine(heart.FormulaNotaId);

                var _base = new FormulaNote
                {
                    NotaId1 = dto.BaseNotes.Note1.Id,
                    NotaId2 = dto.BaseNotes.Note2?.Id,
                    NotaId3 = dto.BaseNotes.Note3?.Id,
                    NotaId4 = dto.BaseNotes.Note4?.Id
                };
                _context.FormulaNotes.Add(_base);
                await _context.SaveChangesAsync();
                Console.WriteLine(_base.FormulaNotaId);

                var formulaConcentration = new FormulaConcentration().CalculateConcentrationBasedOnIntensity(dto.IntensityId);
                Console.WriteLine($"Alcohol: {formulaConcentration.Alcohol}, Agua: {formulaConcentration.Water}, Esencia: {formulaConcentration.Essence}");

                var formula = new Formula
                {
                    FormulaSalida = top.FormulaNotaId,
                    FormulaCorazon = heart.FormulaNotaId,
                    FormulaFondo = _base.FormulaNotaId,
                    IntensidadId = dto.IntensityId,
                    ConcentracionAlcohol = formulaConcentration.Alcohol,
                    ConcentracionAgua = formulaConcentration.Water,
                    ConcentracionEsencia = formulaConcentration.Essence,
                    CreadorId = dto.CreatorId
                };

                _context.Formulas.Add(formula);
                await _context.SaveChangesAsync();

                if (dto.CreatorId != null)
                {
                    var user = await _context.Users.FindAsync(dto.CreatorId);
                    if (user != null)
                    {
                        user.IdFormulas = formula.Id;
                        _context.Users.Update(user);
                        await _context.SaveChangesAsync();
                    }
                }

                await transaction.CommitAsync();
                return formula.Id;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<GETFormulaDTO> GetFormulaByIdToDTOAsync(int id)
        {
            Formula? found = await GetFormulaAsync(id);

            return new GETFormulaDTO
            {
                Id = found.Id,
                Intensity = new IntensityDTO
                {
                    Id = found.IntensidadId,
                    Name = found.Intensidad.Nombre,
                    Description = found.Intensidad.Description,
                    Category = found.Intensidad.Category
                },
                IdCreador = found.CreadorId,
                ConcentracionAlcohol = found.ConcentracionAlcohol,
                ConcentracionAgua = found.ConcentracionAgua,
                ConcentracionEsencia = found.ConcentracionEsencia,
                NotasSalidaIds = MapFormulaNoteToDTO(found.FormulaSalidaNavigation),
                NotasCorazonIds = MapFormulaNoteToDTO(found.FormulaCorazonNavigation),
                NotasFondoIds = MapFormulaNoteToDTO(found.FormulaFondoNavigation),
                Title = found.Title
            };
        }

        public async Task<Formula?> GetFormulaAsync(int id)
        {
            Formula? found = await _context.Formulas
                            .IncludeFormulaNotesWithDetails()
                            .FirstOrDefaultAsync(f => f.Id == id);
            if (found == null)
            {
                throw new KeyNotFoundException();
            }
            return found;
        }

        private GETNoteDTO MapNoteToDTO(Note note)
        {
            return new GETNoteDTO
            {
                Name = note.Name,
                Description = note.Description,
                Family = note.OlfactoryFamily.Nombre,
                Sector = note.OlfactoryPyramid.Sector,
                Duration = note.OlfactoryPyramid.Duracion
            };
        }
        private GETFormulaNoteDTO MapFormulaNoteToDTO(FormulaNote note)
        {
            return new GETFormulaNoteDTO
            {
                Note1 = MapNoteToDTO(note.NotaId1Navigation),
                Note2 = note.NotaId2.HasValue ? MapNoteToDTO(note.NotaId2Navigation!) : null,
                Note3 = note.NotaId3.HasValue ? MapNoteToDTO(note.NotaId3Navigation!) : null,
                Note4 = note.NotaId4.HasValue ? MapNoteToDTO(note.NotaId4Navigation!) : null
            };
        }

        public static byte[] CreatePdf(GETFormulaDTO dto)
        {
            var doc = new PdfDocument();
            var page = doc.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var font = new XFont("Verdana", 12);
            int y = 40;
            gfx.DrawString("Diseño personalizado", new XFont("Verdana", 18, XFontStyle.Bold), XBrushes.Black, new XPoint(40, y));
            y += 40;

            gfx.DrawString($"Notas de fondo: {dto.NotasFondoIds}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Notas de corazón: {dto.NotasCorazonIds}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Notas de salida: {dto.NotasSalidaIds}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Intensidad: {dto.Intensity}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Concentración de alcohol: {dto.ConcentracionAlcohol}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Concentración de agua: {dto.ConcentracionAgua}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            gfx.DrawString($"Concentración de esencia: {dto.ConcentracionEsencia}", font, XBrushes.Black, new XPoint(40, y));
            y += 20;
            using var stream = new MemoryStream();
            doc.Save(stream, false);
            return stream.ToArray();
            throw new NotImplementedException();
        }

        public async Task UpdateTitleAsync(Formula? formula, string title)
        {
            formula.Title = title;
            await _context.SaveChangesAsync();
        }

        public async Task<List<GETFormulaDTO>> GetFormulasByCreatorId(int id)
        {
            var formulas = await _context.Formulas
                        .Where(f => f.CreadorId == id)
                        .Include(f => f.Intensidad)
                        .IncludeFormulaNotesWithDetails()
                        .ToListAsync();


            if (formulas == null || formulas.Count() == 0)
            {
                throw new NullReferenceException();
            }

            return formulas.Select(found => new GETFormulaDTO
            {
                Id = found.Id,
                Intensity = new IntensityDTO
                {
                    Id = found.IntensidadId,
                    Name = found.Intensidad.Nombre,
                    Description = found.Intensidad.Description,
                    Category = found.Intensidad.Category
                },
                IdCreador = found.CreadorId,
                ConcentracionAlcohol = found.ConcentracionAlcohol,
                ConcentracionAgua = found.ConcentracionAgua,
                ConcentracionEsencia = found.ConcentracionEsencia,
                NotasSalidaIds = MapFormulaNoteToDTO(found.FormulaSalidaNavigation),
                NotasCorazonIds = MapFormulaNoteToDTO(found.FormulaCorazonNavigation),
                NotasFondoIds = MapFormulaNoteToDTO(found.FormulaFondoNavigation),
                Title = found.Title
            }).ToList();
        }
    }
}
