using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using alquimia.Services;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
namespace alquimia.Tests.TestServices
{
    public class ChatBotServiceTests
    {
        private readonly Mock<IWebHostEnvironment> _envMock;
        private readonly Mock<IConfiguration> _configMock;
        private readonly List<IChatDynamicNodeHandler> _handlers;

        public ChatBotServiceTests()
        {
            _envMock = new Mock<IWebHostEnvironment>();
            _configMock = new Mock<IConfiguration>();
            _handlers = new List<IChatDynamicNodeHandler>();
        }

        [Fact]
        public async Task GetNodeByIdAsync_ReturnsNode_WhenNodeExists()
        {
            // Arrange
            var nodeId = "node1";
            var jsonContent = $@"
    [
        {{
            ""Id"": ""{nodeId}"",
            ""Message"": ""Hola, soy un nodo de prueba"",
            ""Options"": [
                {{ ""Label"": ""Opción 1"", ""NextNodeId"": ""node2"" }}
            ]
        }}
    ]";

            // Crear carpeta temporal Data
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var dataDir = Path.Combine(tempDir, "Data");
            Directory.CreateDirectory(dataDir);

            var filePath = Path.Combine(dataDir, "chatFlow.json");
            File.WriteAllText(filePath, jsonContent);

            _envMock.Setup(e => e.ContentRootPath).Returns(tempDir);
            _configMock.Setup(c => c["AppSettings:FrontendBaseUrl"]).Returns("https://example.com");

            var service = new ChatbotService(_envMock.Object, _handlers, _configMock.Object);

            // Act
            var node = await service.GetNodeByIdAsync(nodeId);

            // Assert
            Assert.NotNull(node);
            Assert.Equal(nodeId, node.Id);
            Assert.Equal("Hola, soy un nodo de prueba", node.Message);
            Assert.Single(node.Options);
            Assert.Equal("Opción 1", node.Options[0].Label);

            // Cleanup
            Directory.Delete(tempDir, recursive: true);
        }

        [Fact]
        public async Task GetNodeByIdAsync_Throws_WhenNodeDoesNotExist()
        {

            _envMock.Setup(e => e.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _configMock.Setup(c => c["AppSettings:FrontendBaseUrl"]).Returns("https://example.com");


            var service = new ChatbotService(_envMock.Object, _handlers, _configMock.Object);


            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetNodeByIdAsync("inexistente"));
        }

        [Fact]
        public async Task GetDynamicNodeByIdAsync_ReturnsNode_WhenHandlerCanHandle()
        {
            // Arrange
            var dynamicNodeId = "dinamico1";

            var handlerMock = new Mock<IChatDynamicNodeHandler>();
            handlerMock.Setup(h => h.CanHandle(dynamicNodeId)).Returns(true);
            handlerMock.Setup(h => h.HandleAsync(dynamicNodeId))
                .ReturnsAsync(new ChatNode
                {
                    Id = dynamicNodeId,
                    Message = "Soy dinámico",
                    Options = new List<ChatOption>
                    {
                new ChatOption { Label = "Volver", NextNodeId = "start" }
                    }
                });

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var dataDir = Path.Combine(tempDir, "Data");
            Directory.CreateDirectory(dataDir);

            // Crea un JSON válido pero vacío
            var filePath = Path.Combine(dataDir, "chatFlow.json");
            File.WriteAllText(filePath, "[]");

            _envMock.Setup(e => e.ContentRootPath).Returns(tempDir);
            _configMock.Setup(c => c["AppSettings:FrontendBaseUrl"]).Returns("https://example.com");

            var service = new ChatbotService(_envMock.Object, new[] { handlerMock.Object }, _configMock.Object);

            // Act
            var node = await service.GetDynamicNodeByIdAsync(dynamicNodeId);

            // Assert
            Assert.NotNull(node);
            Assert.Equal(dynamicNodeId, node.Id);
            Assert.Equal("Soy dinámico", node.Message);
            Assert.Single(node.Options);
            Assert.Equal("Volver", node.Options[0].Label);

            Directory.Delete(tempDir, recursive: true);
        }


        [Fact]
        public async Task GetDynamicNodeByIdAsync_Throws_WhenNoHandlerCanHandle()
        {
            // Arrange
            var handlerMock = new Mock<IChatDynamicNodeHandler>();
            handlerMock.Setup(h => h.CanHandle(It.IsAny<string>())).Returns(false);

            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            var dataDir = Path.Combine(tempDir, "Data");
            Directory.CreateDirectory(dataDir);

            // Crea un JSON válido pero vacío
            var filePath = Path.Combine(dataDir, "chatFlow.json");
            File.WriteAllText(filePath, "[]");

            _envMock.Setup(e => e.ContentRootPath).Returns(tempDir);
            _configMock.Setup(c => c["AppSettings:FrontendBaseUrl"]).Returns("https://example.com");

            var service = new ChatbotService(_envMock.Object, new[] { handlerMock.Object }, _configMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => service.GetDynamicNodeByIdAsync("dinamicoInexistente"));

            Directory.Delete(tempDir, recursive: true);
        }
    }

