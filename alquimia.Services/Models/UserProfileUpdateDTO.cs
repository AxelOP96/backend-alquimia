using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alquimia.Services.Models
{
    public class UserProfileUpdateDto
    {
        public string? Name { get; set; }
        public string? Empresa { get; set; }
        public string? CUIL { get; set; }
        public string? Rubro { get; set; }
        public string Email { get; set; }
        public bool EsProveedor { get; set; }
    }
}