namespace backendAlquimia.Models
{
    public class POSTFormulaDTO
    {
        public List<int> NotasSalidaIds { get; set; }
        public List<int> NotasCorazonIds { get; set; }
        public List<int> NotasFondoIds { get; set; }
        public int IdIntensidad { get; set; }
        public int IdCreador { get; set; }
    }
}
