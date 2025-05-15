// Data/Entities/PiramideOlfativa.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace backendAlquimia.Data.Entities
{
    public class PiramideOlfativa
    {
        public int Id { get; set; }

        [StringLength(150)]
        public string Nombre { get; set; }

        [StringLength(250)]
        public string Description { get; set; }

        // Ahora Duration es TimeSpan
        public TimeSpan Duracion { get; set; }

        public ICollection<Nota> Notas { get; set; }
    }
}
