using backendAlquimia.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backendAlquimia.Data
{
    public class AlquimiaDbContext : IdentityDbContext<Usuario, Rol, int>
    {
        public AlquimiaDbContext(DbContextOptions<AlquimiaDbContext> options) : base(options)
        {
        }

        // DbSets para cada entidad
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Combinacion> Combinaciones { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<FamiliaOlfativa> FamiliasOlfativas { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<Intensidad> Intensidades { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoProducto> TiposProducto { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<CompatibilidadesFamilias> CompatibilidadesFamilias { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Herencia: Creador y Proveedor extienden Usuario
            modelBuilder.Entity<Usuario>()
                .ToTable("Usuarios");

            modelBuilder.Entity<Usuario>()
                .Property(u => u.Id)
                .HasColumnName("Id");

            // Configuraciones de Combinacion con Nota
            modelBuilder.Entity<Combinacion>()
                .HasMany(c => c.NotaSalida)
                .WithMany()
                .UsingEntity(j => j.ToTable("CombinacionNotaSalida"));

            modelBuilder.Entity<Combinacion>()
                .HasMany(c => c.NotaCorazon)
                .WithMany()
                .UsingEntity(j => j.ToTable("CombinacionNotaCorazon"));

            modelBuilder.Entity<Combinacion>()
                .HasMany(c => c.NotaFondo)
                .WithMany()
                .UsingEntity(j => j.ToTable("CombinacionNotaFondo"));

            // Ajuste para el nombre de columna en Producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.Id)
                .HasColumnName("Id");
            modelBuilder.Entity<Producto>()
            .HasOne(p => p.Proveedor)
            .WithMany(u => u.Productos)
            .HasForeignKey(p => p.IdProveedor)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FamiliaOlfativa>()
            .Property(f => f.Description)
            .HasMaxLength(100);

            modelBuilder.Entity<Nota>()
                .Property(n => n.Descripcion)
                .HasMaxLength(50);

            modelBuilder.Entity<Pedido>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Pedidos");
                entity
                    .HasMany(p => p.Productos)
                    .WithMany();
            });
            modelBuilder.Entity<Formula>()
            .HasOne(f => f.Combinacion)
            .WithMany()
            .HasForeignKey(f => f.CombinacionId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formula>()
            .HasOne(f => f.Intensidad)
            .WithMany(i => i.Formulas)
            .HasForeignKey(f => f.IntensidadId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formula>()
            .HasOne(f => f.Creador)
            .WithMany(c => c.Formulas)
            .HasForeignKey(f => f.CreadorId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nota>()
            .HasOne(n => n.Sector)
            .WithMany(s => s.Notas)
            .HasForeignKey(n => n.SectorId);

            modelBuilder.Entity<CompatibilidadesFamilias>(entity =>
            {
                entity.HasKey(c => c.Id);

                entity.HasOne(c => c.Familia1)
                      .WithMany()
                      .HasForeignKey(c => c.Familia1Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Familia2)
                      .WithMany()
                      .HasForeignKey(c => c.Familia2Id)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.Property(c => c.GradoDeCompatibilidad)
                      .IsRequired();
            });
        }
    }
}