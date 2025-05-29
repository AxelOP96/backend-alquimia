using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Data.Data.Entities;

[Table("Design")]
public partial class Design
{
    [Key]
    public int Id { get; set; }

    public int? IdProducto { get; set; }

    [StringLength(50)]
    public string Text { get; set; } = null!;

    public int? Volume { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string? Image { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? Shape { get; set; }

    [StringLength(10)]
    [Unicode(false)]
    public string? LabelColor { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? Typography { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string? TextColor { get; set; }

    [InverseProperty("Design")]
    public virtual ICollection<FinalEntity> FinalEntities { get; set; } = new List<FinalEntity>();

    [ForeignKey("IdProducto")]
    [InverseProperty("Designs")]
    public virtual Product? IdProductoNavigation { get; set; }
}
