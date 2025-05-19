
using System.ComponentModel.DataAnnotations;

namespace backendAlquimia.Data.Entities
{
    public class FamiliaOlfativa
    {
        public int Id { get; set; }                       

        [StringLength(80)]
        public string Nombre { get; set; }                  
        [MaxLength(100)]
        public string Description {  get; set; }
        public string UrlImagen { get; set; }
    }
}
