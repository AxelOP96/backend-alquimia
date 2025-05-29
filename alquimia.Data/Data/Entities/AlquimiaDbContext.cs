using Microsoft.EntityFrameworkCore;

namespace alquimia.Data.Data.Entities;

public partial class AlquimiaDbContext : DbContext
{
    public AlquimiaDbContext()
    {
    }

    public AlquimiaDbContext(DbContextOptions<AlquimiaDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }

    public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }

    public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }

    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }

    public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }

    public virtual DbSet<Design> Designs { get; set; }

    public virtual DbSet<FamilyCompatibility> FamilyCompatibilities { get; set; }

    public virtual DbSet<FinalEntity> FinalEntities { get; set; }

    public virtual DbSet<Formula> Formulas { get; set; }

    public virtual DbSet<FormulaNote> FormulaNotes { get; set; }

    public virtual DbSet<IncompatibleNote> IncompatibleNotes { get; set; }

    public virtual DbSet<Intensity> Intensities { get; set; }

    public virtual DbSet<Note> Notes { get; set; }

    public virtual DbSet<OlfactoryFamily> OlfactoryFamilies { get; set; }

    public virtual DbSet<OlfactoryPyramid> OlfactoryPyramids { get; set; }

    public virtual DbSet<Option> Options { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderProduct> OrderProducts { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductType> ProductTypes { get; set; }

    public virtual DbSet<ProductVariant> ProductVariants { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Quiz> Quizzes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Subscription> Subscriptions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserProduct> UserProducts { get; set; }

    public virtual DbSet<UserProductReview> UserProductReviews { get; set; }

    public virtual DbSet<UserProviderReview> UserProviderReviews { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=COMPUDEMICA\\SQLEXPRESS;Database=alquimiaDB;Trusted_Connection=True;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AspNetUserClaim>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserClaims).HasConstraintName("FK_AspNetUserClaims_Usuarios_UserId");
        });

        modelBuilder.Entity<AspNetUserLogin>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserLogins).HasConstraintName("FK_AspNetUserLogins_Usuarios_UserId");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId }).HasName("PK_UserRoles");

            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserRoles).HasConstraintName("FK_AspNetUserRoles_Usuarios_UserId");
        });

        modelBuilder.Entity<AspNetUserToken>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.AspNetUserTokens).HasConstraintName("FK_AspNetUserTokens_Usuarios_UserId");
        });

        modelBuilder.Entity<Design>(entity =>
        {
            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Designs).HasConstraintName("FK_design_productId");
        });

        modelBuilder.Entity<FamilyCompatibility>(entity =>
        {
            entity.Property(e => e.FamiliaMayor).HasComputedColumnSql("(case when [Familia1Id]<[Familia2Id] then [Familia2Id] else [Familia1Id] end)", true);
            entity.Property(e => e.FamiliaMenor).HasComputedColumnSql("(case when [Familia1Id]<[Familia2Id] then [Familia1Id] else [Familia2Id] end)", true);

            entity.HasOne(d => d.Familia1).WithMany(p => p.FamilyCompatibilityFamilia1s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompatibilidadesFamilias_FamiliasOlfativas_Familia1Id");

            entity.HasOne(d => d.Familia2).WithMany(p => p.FamilyCompatibilityFamilia2s)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CompatibilidadesFamilias_FamiliasOlfativas_Familia2Id");
        });

        modelBuilder.Entity<FinalEntity>(entity =>
        {
            entity.HasOne(d => d.Design).WithMany(p => p.FinalEntities).HasConstraintName("FK_EntidadFinal_Design");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.FinalEntities).HasConstraintName("FK_EntidadFinal_IdUsuario");

            entity.HasOne(d => d.Productos).WithMany(p => p.FinalEntities).HasConstraintName("FK_EntidadFinal_ProductoId");
        });

        modelBuilder.Entity<Formula>(entity =>
        {
            entity.HasOne(d => d.FormulaCorazonNavigation).WithMany(p => p.FormulaFormulaCorazonNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Formulas_corazon");

            entity.HasOne(d => d.FormulaFondoNavigation).WithMany(p => p.FormulaFormulaFondoNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Formulas_fondo");

            entity.HasOne(d => d.FormulaSalidaNavigation).WithMany(p => p.FormulaFormulaSalidaNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Formulas_salida");

            entity.HasOne(d => d.Intensidad).WithMany(p => p.Formulas)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Formulas_intensidad");
        });

        modelBuilder.Entity<FormulaNote>(entity =>
        {
            entity.Property(e => e.FormulaNotaId).ValueGeneratedNever();

            entity.HasOne(d => d.NotaId1Navigation).WithMany(p => p.FormulaNoteNotaId1Navigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_nota1");

            entity.HasOne(d => d.NotaId2Navigation).WithMany(p => p.FormulaNoteNotaId2Navigations).HasConstraintName("FK_nota2");

            entity.HasOne(d => d.NotaId3Navigation).WithMany(p => p.FormulaNoteNotaId3Navigations).HasConstraintName("FK_nota3");

            entity.HasOne(d => d.NotaId4Navigation).WithMany(p => p.FormulaNoteNotaId4Navigations).HasConstraintName("FK_nota4");
        });

        modelBuilder.Entity<IncompatibleNote>(entity =>
        {
            entity.HasKey(e => new { e.NotaId, e.NotaIncompatibleId }).HasName("PK__Incompat__06842A27C29444C3");

            entity.Property(e => e.NotaMayor).HasComputedColumnSql("(case when [NotaId]<[NotaIncompatibleId] then [NotaIncompatibleId] else [NotaId] end)", true);
            entity.Property(e => e.NotaMenor).HasComputedColumnSql("(case when [NotaId]<[NotaIncompatibleId] then [NotaId] else [NotaIncompatibleId] end)", true);

            entity.HasOne(d => d.Nota).WithMany(p => p.IncompatibleNoteNota)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotaIncompatible_nota");

            entity.HasOne(d => d.NotaIncompatible).WithMany(p => p.IncompatibleNoteNotaIncompatibles)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_NotaIncompatible_notaIncompatible");
        });

        modelBuilder.Entity<Intensity>(entity =>
        {
            entity.Property(e => e.Nombre).HasDefaultValue("");
        });

        modelBuilder.Entity<Note>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Notas");

            entity.HasOne(d => d.FamiliaOlfativa).WithMany(p => p.Notes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_notasFamiliaOlfativa");

            entity.HasOne(d => d.PiramideOlfativa).WithMany(p => p.Notes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_notasPiramideOlfativa");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasOne(d => d.Estado).WithMany(p => p.Orders).HasConstraintName("FK_pedidosEstado");

            entity.HasOne(d => d.Usuario).WithMany(p => p.Orders).HasConstraintName("fk_pedido_usuario");
        });

        modelBuilder.Entity<OrderProduct>(entity =>
        {
            entity.HasOne(d => d.IdPedidoNavigation).WithMany(p => p.OrderProducts).HasConstraintName("FK_pedidoIdPedido");

            entity.HasOne(d => d.Productos).WithMany(p => p.OrderProducts).HasConstraintName("FK_pedidoProductoId");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.Products).HasConstraintName("FK_productIdProv");

            entity.HasOne(d => d.TipoProducto).WithMany(p => p.Products)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ProductTypes");
        });

        modelBuilder.Entity<ProductVariant>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();

            entity.HasOne(d => d.Product).WithMany(p => p.ProductVariants)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductVa__Produ__793DFFAF");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasOne(d => d.IdOpcionesNavigation).WithMany(p => p.Questions).HasConstraintName("FK_QuestionsOptions");
        });

        modelBuilder.Entity<Quiz>(entity =>
        {
            entity.HasOne(d => d.IdPreguntaNavigation).WithMany(p => p.Quizzes).HasConstraintName("FK_quizPreguntas");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_AspNetRoles");
        });

        modelBuilder.Entity<Subscription>(entity =>
        {
            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Subscriptions).HasConstraintName("FK_Status_Subscription");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex")
                .IsUnique()
                .HasFilter("([NormalizedUserName] IS NOT NULL)");

            entity.HasOne(d => d.IdEstadoNavigation).WithMany(p => p.Users).HasConstraintName("FK_estado_Users");

            entity.HasOne(d => d.IdFormulasNavigation).WithMany(p => p.Users).HasConstraintName("FK_formulas_Users");

            entity.HasOne(d => d.IdQuizNavigation).WithMany(p => p.Users).HasConstraintName("FK_quiz_Users");

            entity.HasOne(d => d.IdSuscripcionNavigation).WithMany(p => p.Users).HasConstraintName("FK_suscripcion_Users");
        });

        modelBuilder.Entity<UserProduct>(entity =>
        {
            entity.HasOne(d => d.Producto).WithMany(p => p.UserProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProducts_producto");

            entity.HasOne(d => d.Usuario).WithMany(p => p.UserProducts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserProducts_usuario");
        });

        modelBuilder.Entity<UserProductReview>(entity =>
        {
            entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.UserProductReviews).HasConstraintName("FK_opinion_ProductoId");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UserProductReviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_opinionUsuario_producto");
        });

        modelBuilder.Entity<UserProviderReview>(entity =>
        {
            entity.HasOne(d => d.IdProveedorNavigation).WithMany(p => p.UserProviderReviewIdProveedorNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_opinionProveedor");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UserProviderReviewIdUsuarioNavigations)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_opinionUsuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
