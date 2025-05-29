using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Data.Data.Entities;

[Table("OlfactoryPyramid")]
public partial class OlfactoryPyramid
{
    [Key]
    public int Id { get; set; }

    public string Sector { get; set; } = null!;

    public TimeOnly Duracion { get; set; }

    [InverseProperty("PiramideOlfativa")]
    public virtual ICollection<Note> Notes { get; set; } = new List<Note>();
}
