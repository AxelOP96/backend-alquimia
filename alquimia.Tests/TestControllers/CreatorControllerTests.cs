using alquimia.Api.Controllers;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using alquimia.Tests.TestUtils;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class CreatorControllerTests
    {
        private readonly Mock<INoteService> _mockNoteService;
        private readonly Mock<IFormulaService> _mockFormulaService;
        private readonly Mock<IOlfactoryFamilyService> _mockOlfactoryFamilyService;
        private readonly Mock<IDesignLabelService> _mockDesignLabelService;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<IEmailTemplateService> _mockEmailTemplate;


        private readonly CreatorController _controller;

        public CreatorControllerTests()
        {
            _mockNoteService = new Mock<INoteService>();
            _mockFormulaService = new Mock<IFormulaService>();
            _mockOlfactoryFamilyService = new Mock<IOlfactoryFamilyService>();
            _mockDesignLabelService = new Mock<IDesignLabelService>();
            _mockEmailService = new Mock<IEmailService>();
            _mockEmailTemplate = new Mock<IEmailTemplateService>();

            _controller = new CreatorController(
                _mockNoteService.Object,
                _mockFormulaService.Object,
                _mockOlfactoryFamilyService.Object,
                _mockDesignLabelService.Object,
                _mockEmailService.Object,
                _mockEmailTemplate.Object
                );
        }

        [Fact]
        public void Create_ReturnsOkWithExpectedMessage()
        {
            var result = _controller.Create();
            string expectedMessage = "Ruta activa";
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMessage, okResult.Value);
        }

        [Fact]
        public void Start_RetursnOkWithExpectedMessage()
        {
            var result = _controller.Start();
            string expectedMessage = "Ruta activa";
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(expectedMessage, okResult.Value);
        }

        [Fact]
        public async Task GetBaseNotes_ReturnsOKWithAListOfBaseNotes()
        {
            string sectorInSpanish = "Fondo";
            _mockNoteService.Setup(s => s.GetNotesGroupedByFamilyAsync(sectorInSpanish)).ReturnsAsync(MockGroupedNotesDataDTO.GetBaseNotesGrouped());

            string sectorInEnglish = "base";
            var result = await _controller.GetNotesBySector(sectorInEnglish);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<NotesGroupedByFamilyDTO>>(okResult.Value);

            Assert.Equal(2, data.Count);
            Assert.Contains(data, g => g.Family == "Terroso");
            Assert.Contains(data, g => g.Family == "Amaderado");
        }

        [Fact]
        public async Task GetTopNotes_ReturnsOKWithAListOfTopNotes()
        {
            string sectorInSpanish = "Salida";
            _mockNoteService.Setup(s => s.GetNotesGroupedByFamilyAsync(sectorInSpanish)).ReturnsAsync(MockGroupedNotesDataDTO.GetTopNotesGrouped());

            string sectorInEnglish = "top";
            var result = await _controller.GetNotesBySector(sectorInEnglish);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<NotesGroupedByFamilyDTO>>(okResult.Value);

            Assert.Equal(2, data.Count);
            Assert.Contains(data, g => g.Family == "Cítrico");
            Assert.Contains(data, g => g.Family == "Alcanforado");
        }

        [Fact]
        public async Task GetHeartNotes_ReturnsOKWithAListOfHeartNotes()
        {
            string sectorInSpanish = "Corazón";
            _mockNoteService.Setup(s => s.GetNotesGroupedByFamilyAsync(sectorInSpanish)).ReturnsAsync(MockGroupedNotesDataDTO.GetHeartNotesGrouped());

            string sectorInEnglish = "heart";
            var result = await _controller.GetNotesBySector(sectorInEnglish);

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var data = Assert.IsType<List<NotesGroupedByFamilyDTO>>(okResult.Value);

            Assert.Equal(2, data.Count);
            Assert.Contains(data, g => g.Family == "Frutal");
            Assert.Contains(data, g => g.Family == "Especiado");
        }

        [Fact]
        public async Task PostCompatibleNotes_ShouldReturnEmptyList_WhenNoSelectedNotes()
        {
            // Arrange
            var dto = new SelectedNotesDTO
            {
                ListaDeIdsSeleccionadas = new List<int>(),
                Sector = "Fondo"
            };

            _mockNoteService.Setup(s => s.GetCompatibleNotesAsync(dto.ListaDeIdsSeleccionadas, dto.Sector))
                            .ReturnsAsync(new List<NotesGroupedByFamilyDTO>());

            // Act
            var result = await _controller.PostCompatibleNotes(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<List<NotesGroupedByFamilyDTO>>(okResult.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task PostCompatibleNotes_ShouldReturnCompatibleNotes_WhenCompatibleNotesExist()
        {
            // Arrange
            var dto = new SelectedNotesDTO
            {
                ListaDeIdsSeleccionadas = new List<int> { 1, 2 },
                Sector = "Corazón"
            };

            var mockCompatibleNotes = new List<NotesGroupedByFamilyDTO>
            {
                new NotesGroupedByFamilyDTO
                {
                    Family = "Frutal",
                    Notes = new List<NoteDTO>
                    {
                        new NoteDTO { Id = 1, Name = "Frutal1", Description = "Frutal1 Description" }
                    }
                }
            };

            _mockNoteService.Setup(s => s.GetCompatibleNotesAsync(dto.ListaDeIdsSeleccionadas, dto.Sector))
                            .ReturnsAsync(mockCompatibleNotes);

            // Act
            var result = await _controller.PostCompatibleNotes(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var data = Assert.IsType<List<NotesGroupedByFamilyDTO>>(okResult.Value);
            Assert.Single(data);
            Assert.Equal("Frutal", data[0].Family);
        }

        [Fact]
        public async Task SaveDesign_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new DesignDTO
            {
                Text = "",
                Volume = 100,
                Shape = "Circle",
                LabelColor = "Red"
            };

            _controller.ModelState.AddModelError("Text", "El campo Text es obligatorio.");

            // Act
            var result = await _controller.SaveDesign(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("Text"));
        }

        [Fact]
        public async Task SaveFormula_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var dto = new POSTFormulaDTO
            {
                IntensityId = 0,
                CreatorId = 1,
                TopNotes = null,
                HeartNotes = null,
                BaseNotes = null
            };

            _controller.ModelState.AddModelError("TopNotes", "TopNotes es obligatorio.");

            // Act
            var result = await _controller.SaveFormula(dto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errors = Assert.IsType<SerializableError>(badRequestResult.Value);
            Assert.True(errors.ContainsKey("TopNotes"));
        }

    }
}
