using System.Threading.Tasks;
using alquimia.Data.Entities;
using alquimia.Services.Extensions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class IncludeFormulaNotesWithDetailsTests
    {
        [Fact]
        public async Task IncludeFormulaNotesWithDetails_LoadsRelatedEntities()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase(databaseName: "IncludeFormulaNotesTest")
                .Options;

            using var context = new AlquimiaDbContext(options);

            var intensity = new Intensity { Id = 1, Nombre = "Baja", Description = "desc", Category = "cat" };
            context.Intensities.Add(intensity);

            var family = new OlfactoryFamily { Id = 1, Nombre = "Familia", Description = "desc" };
            var pyramid = new OlfactoryPyramid { Id = 1, Sector = "Sec", Duracion = new System.TimeOnly(0, 10) };
            context.OlfactoryFamilies.Add(family);
            context.OlfactoryPyramids.Add(pyramid);

            var note = new Note
            {
                Id = 1,
                Name = "Nota1",
                OlfactoryFamilyId = family.Id,
                OlfactoryPyramidId = pyramid.Id,
                Description = "desc"
            };
            context.Notes.Add(note);

            var salida = new FormulaNote { FormulaNotaId = 1, NotaId1 = note.Id };
            var corazon = new FormulaNote { FormulaNotaId = 2, NotaId1 = note.Id };
            var fondo = new FormulaNote { FormulaNotaId = 3, NotaId1 = note.Id };
            context.FormulaNotes.AddRange(salida, corazon, fondo);

            var formula = new Formula
            {
                Id = 1,
                FormulaSalida = salida.FormulaNotaId,
                FormulaCorazon = corazon.FormulaNotaId,
                FormulaFondo = fondo.FormulaNotaId,
                IntensidadId = intensity.Id,
                ConcentracionAlcohol = 1,
                ConcentracionAgua = 1,
                ConcentracionEsencia = 1
            };
            context.Formulas.Add(formula);
            await context.SaveChangesAsync();

            var loaded = await context.Formulas
                .IncludeFormulaNotesWithDetails()
                .FirstAsync(f => f.Id == formula.Id);

            Assert.True(context.Entry(loaded).Reference(f => f.Intensidad).IsLoaded);
            Assert.True(context.Entry(loaded.FormulaSalidaNavigation).Reference(fn => fn.NotaId1Navigation).IsLoaded);
            Assert.True(context.Entry(loaded.FormulaSalidaNavigation.NotaId1Navigation).Reference(n => n.OlfactoryFamily).IsLoaded);
            Assert.True(context.Entry(loaded.FormulaSalidaNavigation.NotaId1Navigation).Reference(n => n.OlfactoryPyramid).IsLoaded);
        }
    }
}
