using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace alquimia.Data.Data.Entities;

public partial class AspNetRole : IdentityRole<int>
{
    [Key]
    public int Id { get; set; }

    [StringLength(256)]
    public string? Name { get; set; }

    [StringLength(256)]
    public string? NormalizedName { get; set; }

    public string? ConcurrencyStamp { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<AspNetRoleClaim> AspNetRoleClaims { get; set; } = new List<AspNetRoleClaim>();

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
