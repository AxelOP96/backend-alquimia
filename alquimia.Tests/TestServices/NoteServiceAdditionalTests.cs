using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class NoteServiceAdditionalTests
    {
        [Fact]
        public async Task GetNoteNamesBySectorAsync_ReturnsDistinctNamesLimitedToTen()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase("GetNoteNamesTest")
                .Options;

            using var context = new AlquimiaDbContext(options);
            var pyramid = new OlfactoryPyramid { Sector = "Salida", Duracion = new TimeOnly(0,1) };
            var family = new OlfactoryFamily { Nombre = "Test", Description = "desc" };
            context.OlfactoryPyramids.Add(pyramid);
            context.OlfactoryFamilies.Add(family);

            for (int i = 0; i < 12; i++)
            {
                var note = new Note
                {
                    Name = "Nota" + (i % 6),
                    OlfactoryPyramid = pyramid,
                    OlfactoryFamily = family,
                    Description = "d"
                };
                context.Notes.Add(note);
            }
            await context.SaveChangesAsync();

            var service = new NoteService(context);

            var result = await service.GetNoteNamesBySectorAsync("Salida");

            Assert.Equal(6, result.Distinct().Count());
            Assert.Equal(6, result.Count);
            Assert.Contains("Nota0", result);
        }
    }
}
