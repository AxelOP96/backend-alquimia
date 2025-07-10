using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace alquimia.Tests.TestServices
{
    public class OlfactoryFamilyServiceTests
    {
        private readonly Mock<AlquimiaDbContext> _mockContext;
        private readonly OlfactoryFamilyService _olfactoryFamilyService;

        public OlfactoryFamilyServiceTests()
        {
            _mockContext = new Mock<AlquimiaDbContext>();
            _olfactoryFamilyService = new OlfactoryFamilyService(_mockContext.Object);
        }


        [Fact]
        public async Task GetOlfactoryFamilyInfoAsync_ShouldReturnOlfactoryFamily_WhenUsingInMemoryDatabase()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase(databaseName: "InMemoryDb")
                .Options;

            using var context = new AlquimiaDbContext(options);
            var service = new OlfactoryFamilyService(context);

            var family = new OlfactoryFamily
            {
                Nombre = "Amaderado",
                Description = "Familia con notas amaderadas.",
                Image1 = "amaderado_image.jpg"
            };

            context.OlfactoryFamilies.Add(family);
            await context.SaveChangesAsync();

            var result = await service.GetOlfactoryFamilyInfoAsync(family.Id);

            Assert.NotNull(result);
            Assert.Equal(family.Id, result.Id);
            Assert.Equal("Amaderado", result.Name);
            Assert.Equal("Familia con notas amaderadas.", result.Description);
            Assert.Equal("amaderado_image.jpg", result.Image1);
        }

        [Fact]
        public async Task CreateOlfactoryFamily_ShouldAddFamily_WhenValidFamilyIsProvided()
        {
            var family = new OlfactoryFamilyDTO
            {
                Name = "Floral",
                Description = "Familia con notas florales.",
                Image1 = "floral_image.jpg"
            };

            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase("InMemoryCreateDb")
                .Options;

            using var context = new AlquimiaDbContext(options);
            var service = new OlfactoryFamilyService(context);

            var familyId = await service.CreateOlfactoryFamilyAsync(family);

            var createdFamily = await context.OlfactoryFamilies.FindAsync(familyId);
            Assert.NotNull(createdFamily);
            Assert.Equal(family.Name, createdFamily.Nombre);
            Assert.Equal(family.Description, createdFamily.Description);
            Assert.Equal(family.Image1, createdFamily.Image1);
        }

        [Fact]
        public async Task UpdateOlfactoryFamily_ShouldUpdateFamily_WhenFamilyExists()
        {
            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase("InMemoryUpdateDb")
                .Options;

            using var context = new AlquimiaDbContext(options);
            var service = new OlfactoryFamilyService(context);

            var family = new OlfactoryFamily
            {
                Nombre = "Frutal",
                Description = "Familia con notas frutales.",
                Image1 = "frutal_image.jpg"
            };

            context.OlfactoryFamilies.Add(family);
            await context.SaveChangesAsync();

            family.Nombre = "Frutal Modificada";
            family.Description = "Familia con notas frutales modificadas.";
            family.Image1 = "frutal_modified_image.jpg";
            await service.UpdateOlfactoryFamilyAsync(family);

            var updatedFamily = await context.OlfactoryFamilies.FindAsync(family.Id);
            Assert.Equal("Frutal Modificada", updatedFamily.Nombre);
            Assert.Equal("Familia con notas frutales modificadas.", updatedFamily.Description);
            Assert.Equal("frutal_modified_image.jpg", updatedFamily.Image1);
        }

        [Fact]
        public async Task CreateOlfactoryFamily_ShouldHandleNullImage_WhenImageIsNotProvided()
        {
            var familyDto = new OlfactoryFamilyDTO
            {
                Name = "Cítrica",
                Description = "Familia con notas cítricas.",
                Image1 = null
            };

            var options = new DbContextOptionsBuilder<AlquimiaDbContext>()
                .UseInMemoryDatabase("InMemoryCreateWithNullImageDb")
                .Options;

            using var context = new AlquimiaDbContext(options);
            var service = new OlfactoryFamilyService(context);

            var familyId = await service.CreateOlfactoryFamilyAsync(familyDto);

            var createdFamily = await context.OlfactoryFamilies.FindAsync(familyId);
            Assert.NotNull(createdFamily);
            Assert.Equal(familyDto.Name, createdFamily.Nombre);
            Assert.Equal(familyDto.Description, createdFamily.Description);
            Assert.Null(createdFamily.Image1);
        }
    }
}
