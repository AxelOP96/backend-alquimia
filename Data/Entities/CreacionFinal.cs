using System.ComponentModel.DataAnnotations;
namespace backendAlquimia.Data.Entities;
public class CreacionFinal
{
    public int Id { get; set; }
    public int IdFormula { get; set; }
    public Formula Formula { get; set; }
    public int IdProducto { get; set; }
    public List<Producto> Productos { get; set; } = new();
    public int CreadorId { get; set; }
    public Usuario Creador { get; set; }
    public int IdPedido { get; set; }
    public Pedido Pedido { get; set; }
    public double ConcentracionAlcohol { get; set; }
    public double ConcentracionAgua { get; set; }
    public double ConcentracionEsencia { get; set; }

}