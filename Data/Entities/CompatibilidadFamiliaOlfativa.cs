using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace backendAlquimia.Data.Entities
{
    /// <summary>
    ///  Relación de compatibilidad entre dos Familias Olfativas.
    ///  Se guarda en un solo sentido (A → B) para evitar duplicados;
    ///  si necesitás la relación inversa, la consultás filtrando (A,B) o (B,A).
    /// </summary>
    [Table("CompatibilidadFamiliasOlfativas")]
    public class CompatibilidadFamiliaOlfativa
    {
        public int Id { get; set; }

        /*───────────────────────────────────────────
         *  Claves foráneas
         *───────────────────────────────────────────*/
        [Required]
        [ForeignKey(nameof(FamiliaOlfativaA))]
        public int FamiliaOlfativaAId { get; set; }

        [Required]
        [ForeignKey(nameof(FamiliaOlfativaB))]
        public int FamiliaOlfativaBId { get; set; }

        /*───────────────────────────────────────────
         *  Navegación
         *───────────────────────────────────────────*/
        public virtual FamiliaOlfativa FamiliaOlfativaA { get; set; } = null!;
        public virtual FamiliaOlfativa FamiliaOlfativaB { get; set; } = null!;
    }
}
