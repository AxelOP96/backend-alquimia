namespace backendAlquimia.Data.Entities
{
    public class Envase
    {
        public int Id { get; set; }
        public string Forma { get; set; }
        public int Volumen { get; set; }
        public int IdEtiqueta { get; set; }
        public Etiqueta Etiqueta { get; set; } = new();
        public string UrlImagen { get; set; }
        public string Descripcion { get; set; }
    }
}
