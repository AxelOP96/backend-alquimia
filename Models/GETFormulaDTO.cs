namespace backendAlquimia.Models
{
    public class GETFormulaDTO
    {
        public int Id { get; set; }
        public List<int> NotasSalidaIds { get; set; }
        public List<int> NotasCorazonIds { get; set; }
        public List<int> NotasFondoIds { get; set; }
        public int IdIntensidad { get; set; }
        public int IdCreador { get; set; }
        public double ConcentracionAlcohol { get; set; }
        public double ConcentracionAgua { get; set; }
        public double ConcentracionEsencia { get; set; }
    }
}
