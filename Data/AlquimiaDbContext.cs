using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using backendAlquimia.Data.Entities;
using backendAlquimia.Data.Seeds;

namespace backendAlquimia.Data
{
    public class AlquimiaDbContext : IdentityDbContext<Usuario, Rol, int>
    {
        public AlquimiaDbContext(DbContextOptions<AlquimiaDbContext> options)
            : base(options) { }

        // ─────────── DbSets ───────────
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Creador> Creadores { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Nota> Notas { get; set; }
        public DbSet<FamiliaOlfativa> FamiliasOlfativas { get; set; }
        public DbSet<CompatibilidadFamiliaOlfativa> CompatibilidadesFamilias { get; set; }
        public DbSet<PiramideOlfativa> Sectores { get; set; }
        public DbSet<Combinacion> Combinaciones { get; set; }
        public DbSet<Intensidad> Intensidades { get; set; }
        public DbSet<Formula> Formulas { get; set; }
        public DbSet<CreacionFinal> CreacionesFinales { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<TipoProducto> TiposProducto { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // — Herencia Usuario/Creador/Proveedor
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Creador>().HasBaseType<Usuario>().ToTable("Creadores");
            modelBuilder.Entity<Proveedor>().HasBaseType<Usuario>().ToTable("Proveedores");

            // — CreacionFinal → Creador / Formula / Pedido
            modelBuilder.Entity<CreacionFinal>()
                .HasOne(cf => cf.Creador)
                .WithMany(c => c.HistorialDeCreaciones)
                .HasForeignKey(cf => cf.CreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CreacionFinal>()
                .HasOne(cf => cf.Formula)
                .WithMany()
                .HasForeignKey(cf => cf.IdFormula)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CreacionFinal>()
                .HasOne(cf => cf.Pedido)
                .WithMany()
                .HasForeignKey(cf => cf.IdPedido)
                .OnDelete(DeleteBehavior.Restrict);

            // — Combinacion ↔ Nota (tablas puente)
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

            // — Producto
            modelBuilder.Entity<Producto>()
                .Property(p => p.Id)
                .HasColumnName("Id");

            // — CompatibilidadFamiliaOlfativa (sin cascada)
            modelBuilder.Entity<CompatibilidadFamiliaOlfativa>(cfg =>
            {
                cfg.HasIndex(e => new { e.FamiliaOlfativaAId, e.FamiliaOlfativaBId })
                   .IsUnique();

                cfg.HasOne(e => e.FamiliaOlfativaA)
                   .WithMany()
                   .HasForeignKey(e => e.FamiliaOlfativaAId)
                   .OnDelete(DeleteBehavior.Restrict);

                cfg.HasOne(e => e.FamiliaOlfativaB)
                   .WithMany()
                   .HasForeignKey(e => e.FamiliaOlfativaBId)
                   .OnDelete(DeleteBehavior.Restrict);
            });

            // — Nota → PiramideOlfativa (Sector)
            modelBuilder.Entity<Nota>()
                .HasOne(n => n.Sector)
                .WithMany(s => s.Notas)
                .HasForeignKey(n => n.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Nota>()
                .Property(n => n.Descripcion)
                .HasMaxLength(150);

            // — Formula → Combinacion / Intensidad / Creador
            modelBuilder.Entity<Formula>()
                .HasOne(f => f.Combinacion)
                .WithMany()
                .HasForeignKey(f => f.CombinacionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formula>()
                .HasOne(f => f.Intensidad)
                .WithMany()
                .HasForeignKey(f => f.IntensidadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Formula>()
                .HasOne(f => f.Creador)
                .WithMany(c => c.Formulas)
                .HasForeignKey(f => f.CreadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // — Pedido → Productos (m:n)
            modelBuilder.Entity<Pedido>()
                .ToTable("Pedidos")
                .HasKey(p => p.Id);

            modelBuilder.Entity<Pedido>()
                .HasMany(p => p.Productos)
                .WithMany();

            // — Seeds
            SeedData.Apply(modelBuilder);
        }
    }
}
