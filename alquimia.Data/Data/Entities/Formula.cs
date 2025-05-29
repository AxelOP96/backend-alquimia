using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Data.Data.Entities;

[Index("IntensidadId", Name = "IX_Formulas_IntensidadId")]
public partial class Formula
{
    [Key]
    public int Id { get; set; }

    public int FormulaCorazon { get; set; }

    public int FormulaSalida { get; set; }

    public int FormulaFondo { get; set; }

    public int IntensidadId { get; set; }

    public double ConcentracionAlcohol { get; set; }

    public double ConcentracionAgua { get; set; }

    public double ConcentracionEsencia { get; set; }

    public int? CreadorId { get; set; }

    [ForeignKey("FormulaCorazon")]
    [InverseProperty("FormulaFormulaCorazonNavigations")]
    public virtual FormulaNote FormulaCorazonNavigation { get; set; } = null!;

    [ForeignKey("FormulaFondo")]
    [InverseProperty("FormulaFormulaFondoNavigations")]
    public virtual FormulaNote FormulaFondoNavigation { get; set; } = null!;

    [ForeignKey("FormulaSalida")]
    [InverseProperty("FormulaFormulaSalidaNavigations")]
    public virtual FormulaNote FormulaSalidaNavigation { get; set; } = null!;

    [ForeignKey("IntensidadId")]
    [InverseProperty("Formulas")]
    public virtual Intensity Intensidad { get; set; } = null!;

    [InverseProperty("IdFormulasNavigation")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
