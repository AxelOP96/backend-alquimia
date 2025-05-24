﻿//using backendAlquimia.alquimia.Data;
//using backendAlquimia.Data.Entities;
//using backendAlquimia.Models;
using backendAlquimia.alquimia.Services.Interfaces;
//using Microsoft.EntityFrameworkCore;

namespace backendAlquimia.alquimia.Services
{
    public class ProductoService : IProductoService
    {
        //private readonly AlquimiaDbContext _context;

        //public ProductoService(AlquimiaDbContext context)
        //{
        //    _context = context;
        //}

        //public async Task<ProductoDTO> CrearProductoAsync(CreateProductoDTO dto, int idProveedor)
        //{
        //    // Validar que el proveedor existe
        //    var proveedor = await _context.Usuarios.FindAsync(idProveedor);
        //    if (proveedor == null)
        //        throw new KeyNotFoundException($"Proveedor con ID {idProveedor} no encontrado");

        //    // Buscar el tipo de producto
        //    var tipoProducto = await _context.TiposProducto
        //        .FirstOrDefaultAsync(t => t.Description.ToLower() == dto.TipoProductoDescription.ToLower());

        //    if (tipoProducto == null)
        //        throw new KeyNotFoundException($"Tipo de producto '{dto.TipoProductoDescription}' no existe");

        //    var producto = new Producto
        //    {
        //        Name = dto.Name,
        //        Description = dto.Description,
        //        Price = dto.Price,
        //        Stock = dto.Stock,
        //        IdTipoProducto = tipoProducto.Id,
        //        TipoProducto = tipoProducto,
        //        IdProveedor = idProveedor,
        //        Proveedor = proveedor // Asignamos el proveedor existente
        //    };

        //    try
        //    {
        //        await _context.Productos.AddAsync(producto);
        //        await _context.SaveChangesAsync();
        //        return await ObtenerProductoPorIdAsync(producto.Id, idProveedor);
        //    }
        //    catch (DbUpdateException ex)
        //    {
        //        // Loggear el error interno
        //        Console.WriteLine($"Error al guardar producto: {ex.InnerException?.Message}");
        //        throw new Exception("Error al guardar el producto en la base de datos");
        //    }
        //}

        //public async Task<List<ProductoDTO>> ObtenerProductosPorProveedorAsync(int idProveedor)
        //{
        //    return await _context.Productos
        //        .Where(p => p.IdProveedor == idProveedor)
        //        .Include(p => p.TipoProducto)
        //         .Select(p => new ProductoDTO
        //         {
        //             Id = p.Id,
        //             Name = p.Name,
        //             Description = p.Description,
        //             Price = p.Price,
        //             Stock = p.Stock,
        //             TipoProducto = p.TipoProducto.Description
        //         }).ToListAsync();
        //}

        //public async Task<ProductoDTO> ObtenerProductoPorIdAsync(int idProducto, int idProveedor)
        //{
        //    return await _context.Productos
        //        .Where(p => p.Id == idProducto && p.IdProveedor == idProveedor)
        //        .Include(p => p.TipoProducto)
        //        .Select(p => new ProductoDTO
        //        {
        //            Id = p.Id,
        //            Name = p.Name,
        //            Description = p.Description,
        //            Price = p.Price,
        //            Stock = p.Stock,
        //            TipoProducto = p.TipoProducto.Description
        //        })
        //        .FirstOrDefaultAsync();
        //}


        //public async Task<bool> EliminarProductoAsync(int idProducto, int idProveedor)
        //{
        //    // Verificar que el producto exista y pertenezca al proveedor
        //    var producto = await _context.Productos
        //        .FirstOrDefaultAsync(p => p.Id == idProducto && p.IdProveedor == idProveedor);

        //    if (producto == null)
        //        return false;

        //    _context.Productos.Remove(producto);
        //    await _context.SaveChangesAsync();
        //    return true;
        //}

        //public async Task<ProductoDTO> ActualizarProductoAsync(int idProducto, UpdateProductoDTO dto, int idProveedor)
        //{
        //    var producto = await _context.Productos
        //        .Include(p => p.TipoProducto)
        //        .FirstOrDefaultAsync(p => p.Id == idProducto && p.IdProveedor == idProveedor);

        //    if (producto == null)
        //        throw new KeyNotFoundException("Producto no encontrado o no pertenece al proveedor");

        //    // Actualizamos solo los campos permitidos
        //    if (dto.Name != null) producto.Name = dto.Name;
        //    if (dto.Description != null) producto.Description = dto.Description;
        //    if (dto.Price.HasValue) producto.Price = dto.Price.Value;
        //    if (dto.Stock.HasValue) producto.Stock = dto.Stock.Value;

        //    // EL TIPO DE PRODUCTO NO SE MODIFICA (se mantiene el original)

        //    await _context.SaveChangesAsync();
        //    return await ObtenerProductoPorIdAsync(producto.Id, idProveedor);
        //}


        //public async Task<object> GetHomeDataAsync(int idProveedor)
        //{
        //    var totalProductos = await _context.Productos
        //        .CountAsync(p => p.IdProveedor == idProveedor);

        //    var stockTotal = await _context.Productos
        //        .Where(p => p.IdProveedor == idProveedor)
        //        .SumAsync(p => p.Stock);

        //    return new
        //    {
        //        TotalProductos = totalProductos,
        //        StockTotal = stockTotal,
        //        UltimosProductos = await _context.Productos
        //            .Where(p => p.IdProveedor == idProveedor)
        //            .OrderByDescending(p => p.Id)
        //            .Take(5)
        //             .Include(p => p.TipoProducto)
        //            .Select(p => new ProductoDTO
        //            {
        //                Id = p.Id,
        //                Name = p.Name,
        //                Description = p.Description,
        //                Price = p.Price,
        //                Stock = p.Stock,
        //                TipoProducto = p.TipoProducto.Description
        //            }).ToListAsync()
        //    };
        //}
    }
}
