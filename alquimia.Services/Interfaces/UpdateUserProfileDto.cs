// Services/Models/UpdateUserProfileDto.cs
namespace alquimia.Services.Models
{
    public class UpdateUserProfileDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Cuil { get; set; }
        public string? Empresa { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? Rubro { get; set; }
    }
}