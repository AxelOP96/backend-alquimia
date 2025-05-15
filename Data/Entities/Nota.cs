using System.ComponentModel.DataAnnotations;

namespace backendAlquimia.Data.Entities
{
    public class Nota
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Nombre { get; set; }

        // FK → FamiliaOlfativa
        public int FamiliaOlfativaId { get; set; }
        public FamiliaOlfativa FamiliaOlfativa { get; set; }

        [StringLength(150)]
        public string Descripcion { get; set; }

        // FK → PiramideOlfativa (Sector)
        public int SectorId { get; set; }
        public PiramideOlfativa Sector { get; set; }
    }
}
