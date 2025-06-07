using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace alquimia.Services.Models
{
    public class SubFamilyDTO
    {
        public string Name { get; set; } = string.Empty;
        public List<string> CompatibleNotes { get; set; } = new();
    }

}
