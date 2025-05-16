using System.ComponentModel.DataAnnotations;
namespace backendAlquimia.Data.Entities;

public class Creador : Usuario
{
    public List<CreacionFinal> HistorialDeCreaciones { get; set; } = new();

    public List<Producto> CarritoDeCompras { get; set; } = new();
    public List<Formula> Formulas { get; set; } = new();
    public List<Nota> NotasPreferidas { get; set; } = new();
    public List<FamiliaOlfativa> FamiliasPrefidas { get; set; } = new();
}