namespace backendAlquimia.Data.Entities
{
    public class Esencia : Producto
    {
        public int Id { get; set; }
        public int IdNota { get; set; }

        public string Descripcion { get; set; }
    }
}
