
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendAlquimia.Data.Entities
{
    public class Nota
    {
        public int Id { get; set; }                         

        [StringLength(50)]
        public string Nombre { get; set; }
        public int FamiliaOlfativaId { get; set; }
        public FamiliaOlfativa FamiliaOlfativa { get; set; }

        [StringLength(100)]
        public string Descripcion { get; set; }
        public PiramideOlfativa Sector { get; set; }
        public int SectorId { get; set; }
        public string UrlImagen { get; set; }
    }
}
