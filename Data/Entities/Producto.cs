using System.ComponentModel.DataAnnotations;


namespace backendAlquimia.Data.Entities;

public class Producto
{
    public int Id { get; set; }
    public int IdTipoProducto { get; set; }
    public TipoProducto TipoProducto { get; set; } = new();
    [MaxLength(30)]
    public string Name { get; set; }
    [MaxLength(50)]
    public string Description { get; set; }
    public float Price { get; set; }
    public int Stock { get; set; }
    public int IdProveedor { get; set; }
    public Usuario Proveedor { get; set; } = new();
}