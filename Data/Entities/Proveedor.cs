namespace backendAlquimia.Data.Entities
{
    public class Proveedor : Usuario
    {
        public int Id { get; set; }
        public List<Producto> Productos { get; set; } = new();
    }
}