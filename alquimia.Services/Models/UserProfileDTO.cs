using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using alquimia.Data.Entities;

namespace alquimia.Services.Models
{
    public class UserProfileDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public bool EsProveedor { get; set; }
        public string? CUIL { get; set; }
        public string? Empresa { get; set; }
        public string? Rubro { get; set; }
        public string? Ubicacion { get; set; }
        public string? CodigoPostal { get; set; }
        public int? IdEstado { get; set; }

        public int? cantidadFavoritos { get; set; }
        public int? cantidadFormulas { get; set; }
    }
}
