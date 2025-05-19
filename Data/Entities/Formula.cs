namespace backendAlquimia.Data.Entities
{
    public class Formula
    {
        public int Id { get; set; }
        public int CombinacionId { get; set; }
        public Combinacion Combinacion { get; set; }
        public int CreadorId { get; set; }
        public Usuario Creador { get; set; }
        public double ConcentracionAlcohol { get; set; }
        public double ConcentracionAgua { get; set; }
        public double ConcentracionEsencia { get; set; }
        public int IntensidadId { get; set; }
        public Intensidad Intensidad { get; set; }

    }
}
