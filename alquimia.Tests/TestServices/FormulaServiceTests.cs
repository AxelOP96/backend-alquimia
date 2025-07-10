using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class FormulaServiceTests
    {
        private readonly AlquimiaDbContext _context;
        private readonly FormulaService _formulaService;

        public FormulaServiceTests()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase("AlquimiaTestDb")
                .Options;

            _context = new AlquimiaDbContext(options);
            _formulaService = new FormulaService(_context);
        }

        [Fact]
        public async Task GetIntensitiesAsync_ShouldReturnIntensities()
        {
            var intensities = new[]
            {
                new Intensity { Id = 1, Nombre = "Baja", Description = "Baja intensidad", Category = "Baja" },
                new Intensity { Id = 2, Nombre = "Media", Description = "Media intensidad", Category = "Media" },
                new Intensity { Id = 3, Nombre = "Alta", Description = "Alta intensidad", Category = "Alta" }
            };

            _context.Intensities.AddRange(intensities);
            await _context.SaveChangesAsync();

            var result = await _formulaService.GetIntensitiesAsync();


            Assert.Equal(3, result.Count);
            Assert.Contains(result, i => i.Name == "Baja");
            Assert.Contains(result, i => i.Name == "Media");
            Assert.Contains(result, i => i.Name == "Alta");
        }

        [Fact]
        public void CreatePdf_ShouldGeneratePdfFile()
        {

            var dto = new GETFormulaDTO
            {
                Id = 1,
                NotasSalidaIds = new GETFormulaNoteDTO(),
                NotasCorazonIds = new GETFormulaNoteDTO(),
                NotasFondoIds = new GETFormulaNoteDTO(),
                Intensity = new IntensityDTO { Name = "Baja" },
                Title = "Test Formula",
                ConcentracionAlcohol = 70.0,
                ConcentracionAgua = 27.0,
                ConcentracionEsencia = 3.0
            };


            var result = FormulaService.CreatePdf(dto);


            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }

        [Fact]
        public async Task UpdateTitleAsync_ShouldUpdateTitle()
        {

            var formula = new Formula { Id = 1, Title = "Old Title" };
            _context.Formulas.Add(formula);
            await _context.SaveChangesAsync();

            var newTitle = "New Title";


            await _formulaService.UpdateTitleAsync(formula, newTitle);


            var updatedFormula = await _context.Formulas.FindAsync(formula.Id);
            Assert.Equal(newTitle, updatedFormula?.Title);
        }

        [Fact]
        public async Task GetFormulaByIdToDTOAsync_ShouldThrowKeyNotFoundException_WhenFormulaNotFound()
        {
            var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _formulaService.GetFormulaByIdToDTOAsync(999));

            Assert.Equal("The given key was not present in the dictionary.", result.Message);
        }

        [Fact]
        public async Task GetFormulaByIdToDTOAsync_ShouldThrowKeyNotFoundException_WhenFormulaDoesNotExist()
        {
            var result = await Assert.ThrowsAsync<KeyNotFoundException>(() =>
                _formulaService.GetFormulaByIdToDTOAsync(999));

            Assert.Equal("The given key was not present in the dictionary.", result.Message);
        }

        [Fact]
        public void CreatePdf_ShouldGeneratePdfFileCorrectly()
        {
            var dto = new GETFormulaDTO
            {
                Id = 1,
                NotasSalidaIds = new GETFormulaNoteDTO(),
                NotasCorazonIds = new GETFormulaNoteDTO(),
                NotasFondoIds = new GETFormulaNoteDTO(),
                Intensity = new IntensityDTO { Name = "Baja" },
                Title = "Test Formula",
                ConcentracionAlcohol = 70.0,
                ConcentracionAgua = 27.0,
                ConcentracionEsencia = 3.0
            };

            var result = FormulaService.CreatePdf(dto);

            Assert.NotNull(result);
            Assert.True(result.Length > 0);
        }
    }
}
