namespace backendAlquimia.Data.Entities
{
    public class PiramideOlfativa
    {
        public int Id {  get; set; }
        public string Nombre { get; set; }
        public List<Nota> Notas { get; set; } = new List<Nota>();
        public TimeSpan Duracion { get; set; }
        public string UrlImagen { get; set; }
    }
}
