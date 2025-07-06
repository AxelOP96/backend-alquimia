using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using alquimia.Services.Handler;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Moq;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class DynamicHandlersTests
    {
        [Fact]
        public async Task TopNotesHandler_ReturnsChatNodeWithNoteNames()
        {
            var noteServiceMock = new Mock<INoteService>();
            noteServiceMock.Setup(n => n.GetNoteNamesBySectorAsync("Salida"))
                .ReturnsAsync(new List<string> { "A", "B", "C" });

            var handler = new DinamicTopNotesHandler(noteServiceMock.Object);
            Assert.True(handler.CanHandle("aprendizaje-notas-salida-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-notas-salida-dinamico");

            Assert.NotNull(node);
            Assert.Equal("aprendizaje-notas-salida-dinamico", node!.Id);
            Assert.Contains("A, B, C", node.Message);
            Assert.Equal(2, node.Options.Count);
        }

        [Fact]
        public async Task HeartNotesHandler_ReturnsChatNodeWithNoteNames()
        {
            var noteServiceMock = new Mock<INoteService>();
            noteServiceMock.Setup(n => n.GetNoteNamesBySectorAsync("Corazón"))
                .ReturnsAsync(new List<string> { "H1", "H2", "H3" });

            var handler = new DinamicHeartNotesHandler(noteServiceMock.Object);
            Assert.True(handler.CanHandle("aprendizaje-notas-corazon-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-notas-corazon-dinamico");

            Assert.NotNull(node);
            Assert.Contains("H1, H2, H3", node!.Message);
        }

        [Fact]
        public async Task BaseNotesHandler_ReturnsChatNodeWithNoteNames()
        {
            var noteServiceMock = new Mock<INoteService>();
            noteServiceMock.Setup(n => n.GetNoteNamesBySectorAsync("Fondo"))
                .ReturnsAsync(new List<string> { "B1", "B2", "B3" });

            var handler = new DinamicBaseNotesHandler(noteServiceMock.Object);
            Assert.True(handler.CanHandle("aprendizaje-notas-fondo-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-notas-fondo-dinamico");

            Assert.NotNull(node);
            Assert.Contains("B1, B2, B3", node!.Message);
        }

        [Fact]
        public async Task NotesHandler_ReturnsChatNodeWithOptions()
        {
            var handler = new DinamicNotesHandler();

            Assert.True(handler.CanHandle("aprendizaje-notas-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-notas-dinamico");

            Assert.NotNull(node);
            Assert.Equal("aprendizaje-notas-dinamico", node!.Id);
            Assert.Contains("nota", node.Message, StringComparison.InvariantCultureIgnoreCase);
            Assert.Equal(5, node.Options.Count);
        }

        [Fact]
        public async Task IntensitiesHandler_ReturnsChatNodeWithIntensities()
        {
            var formulaServiceMock = new Mock<IFormulaService>();
            formulaServiceMock.Setup(f => f.GetIntensitiesAsync())
                .ReturnsAsync(new List<IntensityDTO>
                {
                    new IntensityDTO { Name = "Baja", Description = "desc", Category = "cat" },
                    new IntensityDTO { Name = "Alta", Description = "otra", Category = "cat" }
                });

            var handler = new DinamicIntensitiesHandler(formulaServiceMock.Object);
            Assert.True(handler.CanHandle("aprendizaje-intensidades-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-intensidades-dinamico");

            Assert.NotNull(node);
            Assert.Contains("Baja", node!.Message);
            Assert.Contains("Alta", node.Message);
            Assert.Equal(2, node.Options.Count);
        }

        [Fact]
        public async Task FamilyHandler_ReturnsChatNodeWithFamilies()
        {
            var serviceMock = new Mock<IOlfactoryFamilyService>();
            serviceMock.Setup(s => s.GetAllFamilies())
                .ReturnsAsync(new List<OlfactoryFamilyDTO>
                {
                    new OlfactoryFamilyDTO { Name = "Cítrica", Description = "desc" },
                    new OlfactoryFamilyDTO { Name = "Floral", Description = "desc" }
                });

            var handler = new DinamicFamilyHandler(serviceMock.Object);
            Assert.True(handler.CanHandle("aprendizaje-familias-dinamico"));

            var node = await handler.HandleAsync("aprendizaje-familias-dinamico");

            Assert.NotNull(node);
            Assert.Contains("Cítrica", node!.Message);
            Assert.Contains("Floral", node.Message);
            Assert.Equal(2, node.Options.Count);
        }

        [Fact]
        public async Task ProviderHelpHandler_ReturnsInputNode()
        {
            var handler = new DinamicStateProviderHelp();

            Assert.True(handler.CanHandle("proveedor-ayuda-estado-dinamico"));

            var node = await handler.HandleAsync("proveedor-ayuda-estado-dinamico");

            Assert.NotNull(node);
            Assert.Equal("email", node!.InputType);
            Assert.Equal("proveedor-ayuda-estado-respuesta-dinamico", node.NextNodeId);
        }

        [Fact]
        public async Task ProviderHelpResponse_NoEmail_ReturnsErrorNode()
        {
            var adminMock = new Mock<IAdminService>();
            var handler = new DinamicStateProviderHelpResponse(adminMock.Object);

            var node = await handler.HandleAsync("proveedor-ayuda-estado-respuesta-dinamico");

            Assert.NotNull(node);
            Assert.Contains("No se recibió", node!.Message);
            Assert.Equal(2, node.Options.Count);
        }

        [Fact]
        public async Task ProviderHelpResponse_WithEmail_ReturnsState()
        {
            var adminMock = new Mock<IAdminService>();
            adminMock.Setup(a => a.GetPendingOrApprovedProviderByEmailAsync("user@test.com"))
                .ReturnsAsync(new ProviderDTO { EsAprobado = true });

            var handler = new DinamicStateProviderHelpResponse(adminMock.Object);

            var node = await handler.HandleAsync("proveedor-ayuda-estado-respuesta-dinamico::user@test.com");

            Assert.NotNull(node);
            Assert.Contains("aprobado", node!.Message);
            Assert.Equal(2, node.Options.Count);
        }
    }
}
