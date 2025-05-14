
namespace backendAlquimia.Models
{
    public class NotaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Familia { get; internal set; }
        public string Sector { get; internal set; }
        public string Descripcion { get; set; }
        public TimeSpan Duracion { get; internal set; }
    }
}
