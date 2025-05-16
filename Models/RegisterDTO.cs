namespace backendAlquimia.Models
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Rol { get; set; } // "Admin", "Creador", "Proveedor"

    }
}
