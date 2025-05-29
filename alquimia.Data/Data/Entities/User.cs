using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Data.Data.Entities;

[Index("NormalizedEmail", Name = "EmailIndex")]
public partial class User
{
    [Key]
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? IdEstado { get; set; }

    public int? IdFormulas { get; set; }

    public int? IdQuiz { get; set; }

    public int? IdSuscripcion { get; set; }

    public bool EsProveedor { get; set; }

    [StringLength(256)]
    public string? UserName { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    public bool EmailConfirmed { get; set; }

    public string? PasswordHash { get; set; }

    public string? SecurityStamp { get; set; }

    public string? ConcurrencyStamp { get; set; }

    public bool TwoFactorEnabled { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }

    public bool LockoutEnabled { get; set; }

    public int AccessFailedCount { get; set; }

    [StringLength(256)]
    public string? NormalizedEmail { get; set; }

    [StringLength(256)]
    public string? NormalizedUserName { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    public bool PhoneNumberConfirmed { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; } = new List<AspNetUserClaim>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; } = new List<AspNetUserLogin>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserRole> AspNetUserRoles { get; set; } = new List<AspNetUserRole>();

    [InverseProperty("User")]
    public virtual ICollection<AspNetUserToken> AspNetUserTokens { get; set; } = new List<AspNetUserToken>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<FinalEntity> FinalEntities { get; set; } = new List<FinalEntity>();

    [ForeignKey("IdEstado")]
    [InverseProperty("Users")]
    public virtual Status? IdEstadoNavigation { get; set; }

    [ForeignKey("IdFormulas")]
    [InverseProperty("Users")]
    public virtual Formula? IdFormulasNavigation { get; set; }

    [ForeignKey("IdQuiz")]
    [InverseProperty("Users")]
    public virtual Quiz? IdQuizNavigation { get; set; }

    [ForeignKey("IdSuscripcion")]
    [InverseProperty("Users")]
    public virtual Subscription? IdSuscripcionNavigation { get; set; }

    [InverseProperty("Usuario")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UserProductReview> UserProductReviews { get; set; } = new List<UserProductReview>();

    [InverseProperty("Usuario")]
    public virtual ICollection<UserProduct> UserProducts { get; set; } = new List<UserProduct>();

    [InverseProperty("IdProveedorNavigation")]
    public virtual ICollection<UserProviderReview> UserProviderReviewIdProveedorNavigations { get; set; } = new List<UserProviderReview>();

    [InverseProperty("IdUsuarioNavigation")]
    public virtual ICollection<UserProviderReview> UserProviderReviewIdUsuarioNavigations { get; set; } = new List<UserProviderReview>();
}
