using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace backendAlquimia.Data.Entities;


public class Usuario : IdentityUser<int>
{
    public List<Opinion> Opiniones { get; set; } = new();
    public string Name { get; set; }

    public bool EsProveedor { get; set; } = false;
    public List<CreacionFinal>? HistorialDeCreaciones { get; set; }
    public List<Producto>? CarritoDeCompras { get; set; }
    public List<Formula>? Formulas { get; set; }
    public List<Nota>? NotasPreferidas { get; set; }
    public List<FamiliaOlfativa>? FamiliasPreferidas { get; set; }

    public List<Producto>? Productos { get; set; } // Si es proveedor

}
