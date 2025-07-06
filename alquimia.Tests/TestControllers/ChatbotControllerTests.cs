using alquimia.Api.Controllers;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace alquimia.Tests.TestControllers
{
    public class ChabotControllerTests
    {
        private readonly Mock<IChatbotService> _serviceMock;
        private readonly ChatbotController _controller;

        public ChabotControllerTests()
        {
            _serviceMock = new Mock<IChatbotService>();
            _controller = new ChatbotController(_serviceMock.Object);
        }

        [Fact]
        public async Task Start_ReturnsInicioNode()
        {
            var inicioNode = new ChatNode { Id = "inicio", Message = "¡Hola! ¿Con qué te puedo ayudar?" };
            _serviceMock.Setup(s => s.GetNodeByIdAsync("inicio"))
                       .ReturnsAsync(inicioNode);

            var result = await _controller.Start() as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(inicioNode, result.Value);
        }

        [Fact]
        public async Task GetNode_ReturnsOk_WhenAStaticExistentNodeIsRequested()
        {
            var learningNode = new ChatNode
            {
                Id = "aprendizaje",
                Message = "¿Qué te gustaría aprender sobre el mundo de las fragancias?"
            };

            _serviceMock.Setup(s => s.GetNodeByIdAsync("aprendizaje")).ReturnsAsync(learningNode);

            var result = await _controller.GetNode("aprendizaje") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(learningNode, result.Value);
        }

        [Fact]
        public async Task GetNode_ReturnsOk_WhenADynamicExistentNodeIsRequested()
        {
            var familiesNode = new ChatNode
            {
                Id = "aprendizaje-familias-dinamico",
                Message = "¿Qué te gustaría aprender sobre las familias olfativas?"
            };

            _serviceMock.Setup(s => s.GetNodeByIdAsync("aprendizaje-familias-dinamico")).ThrowsAsync(new KeyNotFoundException());

            _serviceMock.Setup(s => s.GetDynamicNodeByIdAsync("aprendizaje-familias-dinamico")).ReturnsAsync(familiesNode);

            var result = await _controller.GetNode("aprendizaje-familias-dinamico") as OkObjectResult;

            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(familiesNode, result.Value);
        }

        [Fact]
        public async Task GetNode_ThrowsKeyNotFoundException_WhenNodeNotFoundAnywhere()
        {
            var nodeId = "inexistente";

            _serviceMock.Setup(s => s.GetNodeByIdAsync(nodeId))
                        .ThrowsAsync(new KeyNotFoundException());

            _serviceMock.Setup(s => s.GetDynamicNodeByIdAsync(nodeId))
                        .ThrowsAsync(new KeyNotFoundException());

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _controller.GetNode(nodeId));
        }
        //GetNode returns a dinamic node when a dynamic node is requested
        //GetNode does not return static node when a dynamic node is requested
        //GetNode throws KeyNotFound when StaticNode Not Found SERVICE
    }
}
