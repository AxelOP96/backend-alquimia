// Data/Seeds/SeedData.cs
using System;
using Microsoft.EntityFrameworkCore;
using backendAlquimia.Data.Entities;

namespace backendAlquimia.Data.Seeds
{
    public static class SeedData
    {
        public static void Apply(ModelBuilder modelBuilder)
        {
            /*──────────────────────────────────
             * 0) PIRÁMIDE OLFATIVA (SECTORES)
             *──────────────────────────────────*/
            modelBuilder.Entity<PiramideOlfativa>().HasData(
                new PiramideOlfativa
                {
                    Id = 1,
                    Nombre = "Salida",
                    Description = "Notas que se perciben inmediatamente.",
                    Duracion = new TimeSpan(0, 60, 0)   // 60 minutos
                },
                new PiramideOlfativa
                {
                    Id = 2,
                    Nombre = "Corazón",
                    Description = "Notas que definen el carácter del perfume.",
                    Duracion = new TimeSpan(0, 240, 0)  // 240 minutos (4h)
                },
                new PiramideOlfativa
                {
                    Id = 3,
                    Nombre = "Fondo",
                    Description = "Notas de base, las más persistentes.",
                    Duracion = new TimeSpan(0, 480, 0)  // 480 minutos (8h)
                }
            );

            /*───────────────────────────────
             * 1) FAMILIAS OLFATIVAS
             *──────────────────────────────*/
            modelBuilder.Entity<FamiliaOlfativa>().HasData(
                new FamiliaOlfativa { Id = 1, Nombre = "Cítrica", Description = "Notas frescas y chispeantes derivadas de cítricos." },
                new FamiliaOlfativa { Id = 2, Nombre = "Frutal", Description = "Acordes dulces y jugosos de frutas no cítricas." },
                new FamiliaOlfativa { Id = 3, Nombre = "Floral", Description = "Bouquet de flores de tallo, pétalos y capullos." },
                new FamiliaOlfativa { Id = 4, Nombre = "Oriental", Description = "Acordes especiados, resinosos y cálidos." },
                new FamiliaOlfativa { Id = 5, Nombre = "Amaderada", Description = "Esencias de maderas secas, resinosas o ahumadas." },
                new FamiliaOlfativa { Id = 6, Nombre = "Aromática", Description = "Hierbas y plantas de aroma fresco y limpio." },
                new FamiliaOlfativa { Id = 7, Nombre = "Verde", Description = "Hojas y tallos recién cortados, sensación herbal." },
                new FamiliaOlfativa { Id = 8, Nombre = "Gourmand", Description = "Notas comestibles, dulces y reconfortantes." },
                new FamiliaOlfativa { Id = 9, Nombre = "Almizclada", Description = "Acordes suaves, limpios y almizclados." },
                new FamiliaOlfativa { Id = 10, Nombre = "Acuática", Description = "Sensación marina y de brisa fresca y húmeda." }
            );

            /*───────────────────────────────
             * 2) NOTAS / ESENCIAS
             *──────────────────────────────*/
            modelBuilder.Entity<Nota>().HasData(
                // — CÍTRICAS (Sector Salida)
                new Nota { Id = 1, Nombre = "Bergamota", Descripcion = "Cítrico verde, chispeante y ligeramente floral.", FamiliaOlfativaId = 1, SectorId = 1 },
                new Nota { Id = 2, Nombre = "Limón", Descripcion = "Cítrico luminoso y ácido, recuerda a la piel de limón.", FamiliaOlfativaId = 1, SectorId = 1 },
                new Nota { Id = 3, Nombre = "Mandarina", Descripcion = "Cítrico dulce y jugoso, con matiz infantil y alegre.", FamiliaOlfativaId = 1, SectorId = 1 },
                // — FRUTALES (Sector Salida)
                new Nota { Id = 4, Nombre = "Manzana", Descripcion = "Frutal crujiente, faceta verde-dulce que aporta frescura.", FamiliaOlfativaId = 2, SectorId = 1 },
                new Nota { Id = 5, Nombre = "Frambuesa", Descripcion = "Fruta roja ácida y azucarada, da un tono juvenil.", FamiliaOlfativaId = 2, SectorId = 1 },
                // — FLORALES (Sector Corazón)
                new Nota { Id = 6, Nombre = "Rosa", Descripcion = "Floral clásico, aterciopelado y ligeramente especiado.", FamiliaOlfativaId = 3, SectorId = 2 },
                new Nota { Id = 7, Nombre = "Jazmín", Descripcion = "Floral blanco, opulento, con matiz indólico y sensual.", FamiliaOlfativaId = 3, SectorId = 2 },
                // — ORIENTALES (Sector Fondo)
                new Nota { Id = 8, Nombre = "Vainilla", Descripcion = "Dulce, cremosa y envolvente; aporta calidez oriental.", FamiliaOlfativaId = 4, SectorId = 3 },
                new Nota { Id = 9, Nombre = "Ámbar", Descripcion = "Acorde resinoso-balsámico, aporta profundidad y dulzor.", FamiliaOlfativaId = 4, SectorId = 3 },
                // — AMADERADAS (Sector Fondo)
                new Nota { Id = 10, Nombre = "Sándalo", Descripcion = "Madera lactónica, suave y cremosa con faceta almizclada.", FamiliaOlfativaId = 5, SectorId = 3 },
                new Nota { Id = 11, Nombre = "Cedro", Descripcion = "Madera seca, terrosa y ligeramente ahumada.", FamiliaOlfativaId = 5, SectorId = 3 },
                // — AROMÁTICAS (Sector Corazón)
                new Nota { Id = 12, Nombre = "Lavanda", Descripcion = "Aromática herbal, calmante y limpia (toque alcanforado).", FamiliaOlfativaId = 6, SectorId = 2 },
                new Nota { Id = 13, Nombre = "Salvia", Descripcion = "Aromática verde con punto terroso y fresco.", FamiliaOlfativaId = 6, SectorId = 2 },
                // — VERDES (Sector Salida)
                new Nota { Id = 14, Nombre = "Hoja de Higuera", Descripcion = "Verde lechoso, natural y ligeramente afrutado.", FamiliaOlfativaId = 7, SectorId = 1 },
                // — GOURMAND (Sector Fondo)
                new Nota { Id = 15, Nombre = "Chocolate", Descripcion = "Cacao profundo, cremoso y envolvente, efecto gourmand.", FamiliaOlfativaId = 8, SectorId = 3 },
                // — ALMIZCLADAS (Sector Fondo)
                new Nota { Id = 16, Nombre = "Almizcle Blanco", Descripcion = "Suave, limpio y empolvado; fija y da sensación de piel.", FamiliaOlfativaId = 9, SectorId = 3 },
                // — ACUÁTICAS (Sector Salida)
                new Nota { Id = 17, Nombre = "Brisa Marina", Descripcion = "Acorde ozónico-salado que evoca aire marino fresco.", FamiliaOlfativaId = 10, SectorId = 1 }
            );

            /*───────────────────────────────
             * 3) COMPATIBILIDAD ENTRE FAMILIAS
             *──────────────────────────────*/
            modelBuilder.Entity<CompatibilidadFamiliaOlfativa>().HasData(
                new CompatibilidadFamiliaOlfativa { Id = 1, FamiliaOlfativaAId = 1, FamiliaOlfativaBId = 3 },
                new CompatibilidadFamiliaOlfativa { Id = 2, FamiliaOlfativaAId = 1, FamiliaOlfativaBId = 6 },
                new CompatibilidadFamiliaOlfativa { Id = 3, FamiliaOlfativaAId = 2, FamiliaOlfativaBId = 3 },
                new CompatibilidadFamiliaOlfativa { Id = 4, FamiliaOlfativaAId = 3, FamiliaOlfativaBId = 5 },
                new CompatibilidadFamiliaOlfativa { Id = 5, FamiliaOlfativaAId = 4, FamiliaOlfativaBId = 5 },
                new CompatibilidadFamiliaOlfativa { Id = 6, FamiliaOlfativaAId = 4, FamiliaOlfativaBId = 9 },
                new CompatibilidadFamiliaOlfativa { Id = 7, FamiliaOlfativaAId = 8, FamiliaOlfativaBId = 4 },
                new CompatibilidadFamiliaOlfativa { Id = 8, FamiliaOlfativaAId = 10, FamiliaOlfativaBId = 1 }
            );
        }
    }
}
