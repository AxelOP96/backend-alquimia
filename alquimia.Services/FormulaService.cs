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
                var top = FormulaNoteHelper.CreateFormulaNote(dto.TopNotes);
                var heart = FormulaNoteHelper.CreateFormulaNote(dto.HeartNotes);
                var _base = FormulaNoteHelper.CreateFormulaNote(dto.BaseNotes);
                _context.FormulaNotes.AddRange(top, heart, _base);
                await _context.SaveChangesAsync();
                var formulaConcentration = new FormulaConcentrationHelper().CalculateConcentrationBasedOnIntensity(dto.IntensityId);
                Formula formula = CreateFormulaEntity(dto, top, heart, _base, formulaConcentration);
                _context.Formulas.Add(formula);
                await _context.SaveChangesAsync();
                if (dto.CreatorId != null)
                {
                    var user = await _context.Users.FindAsync(dto.CreatorId);
                    if (user != null)
                    {
                        formula.Creator = user;
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

        private static Formula CreateFormulaEntity(POSTFormulaDTO dto, FormulaNote top, FormulaNote heart, FormulaNote _base, FormulaConcentrationHelper formulaConcentration)
        {
            return new Formula
            {
                FormulaSalida = top.FormulaNotaId,
                FormulaCorazon = heart.FormulaNotaId,
                FormulaFondo = _base.FormulaNotaId,
                IntensidadId = dto.IntensityId,
                ConcentracionAlcohol = formulaConcentration.Alcohol,
                ConcentracionAgua = formulaConcentration.Water,
                ConcentracionEsencia = formulaConcentration.Essence,
                CreatorId = dto.CreatorId
            };
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
                CreatorId = found.CreatorId,
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
                Duration = note.OlfactoryPyramid.Duracion,
                Image = note.Image
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
    }
}
