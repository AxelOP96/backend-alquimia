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
            modelBuilder.Entity<Formula>(entity =>
            {
                entity.ToTable("Formulas");

                // Clave primaria
                entity.HasKey(f => f.Id);

                // -------- Propiedades de valor ----------------------------------------
                entity.Property(f => f.ConcentracionAlcohol)
                      .HasPrecision(5, 2)          // Ej.: 100.00 %
                      .IsRequired();

                entity.Property(f => f.ConcentracionAgua)
                      .HasPrecision(5, 2)
                      .IsRequired();

                entity.Property(f => f.ConcentracionEsencia)
                      .HasPrecision(5, 2)
                      .IsRequired();

                // Si IntensidadDatoCalculado es solo de cálculo, no lo mapeamos
                entity.Ignore(f => f.IntensidadDatoCalculado);

                // -------- Relaciones --------------------------------------------------

                // Combinación (1-N)  ▸ una Combinacion puede tener muchas fórmulas
                entity.HasOne(f => f.Combinacion)
                      .WithMany(c => c.Formulas)         // agrega la colección c.Formulas
                      .HasForeignKey(f => f.CombinacionId)
                      .OnDelete(DeleteBehavior.Restrict) // no cascada
                      .IsRequired();

                // Creador (Usuario)  ▸ un usuario puede crear muchas fórmulas
                entity.HasOne(f => f.Creador)
            .WithMany(u => u.Formulas)  // Use the existing Formulas collection
            .HasForeignKey(f => f.CreadorId)
            .OnDelete(DeleteBehavior.Restrict)
            .IsRequired();
                // Remove this incorrect relationship configuration
                // entity.HasOne(f => f.IntensidadDatoCalculado)
                //      .WithMany()
                //      .HasForeignKey(f => f.IntensidadId)
                //      .OnDelete(DeleteBehavior.Restrict)
                //      .IsRequired();

                // Instead, just configure it as a regular property if needed
                entity.Property(f => f.IntensidadDatoCalculado)
                      .IsRequired();

                // -------- Índices / restricciones únicas -----------------------------
                // Evita que exista más de una fórmula con la misma combinación + creador
                entity.HasIndex(f => new { f.CombinacionId, f.CreadorId })
                      .IsUnique();
            });


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