﻿using backendAlquimia.Data;
using backendAlquimia.Models;
using backendAlquimia.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace backendAlquimia.Controllers
{
    [Authorize]
    [Route("api/proveedor")]
    [ApiController]
    public class ProveedorController : ControllerBase
    {
        private readonly IProductoService _productoService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AlquimiaDbContext _context;
     

        public ProveedorController(
            IProductoService productoService,
            IHttpContextAccessor httpContextAccessor,
            AlquimiaDbContext context)
        {
            _productoService = productoService;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        private int ObtenerIdProveedor()
        {
            var userIdClaim = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        [HttpGet("home")]
        public async Task<IActionResult> GetHomeData()
        {
            var idProveedor = ObtenerIdProveedor();
            var data = await _productoService.GetHomeDataAsync(idProveedor);
            return Ok(data);
        }

        [HttpGet("productos")]
        public async Task<IActionResult> GetProductos()
        {
            var idProveedor = ObtenerIdProveedor();
            var productos = await _productoService.ObtenerProductosPorProveedorAsync(idProveedor);
            return Ok(productos);
        }

        [HttpGet("tipos-producto")]
        public async Task<IActionResult> GetTiposProducto()
        {
            var tipos = await _context.TiposProducto
                .Select(t => new
                {
                    Descripcion = t.Description
                })
                .ToListAsync();

            return Ok(tipos);
        }


        [HttpPost("productos")]
        public async Task<IActionResult> CrearProducto([FromBody] CreateProductoDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    mensaje = "Datos inválidos",
                    errores = ModelState.Values.SelectMany(v => v.Errors)
                });
            }

            try
            {
                var idProveedor = ObtenerIdProveedor();
                var producto = await _productoService.CrearProductoAsync(dto, idProveedor);
                return CreatedAtAction(nameof(GetProducto), new { idProducto = producto.Id }, producto);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                // Loggear el error para diagnóstico
                Console.WriteLine($"Error al crear producto: {ex}");
                return StatusCode(500, new
                {
                    mensaje = "Error interno al crear el producto",
                    detalle = ex.Message
                });
            }
        }

        [HttpGet("productos/{idProducto}")]
        public async Task<IActionResult> GetProducto(int idProducto)
        {
            var idProveedor = ObtenerIdProveedor();
            var producto = await _productoService.ObtenerProductoPorIdAsync(idProducto, idProveedor);

            if (producto == null)
                return NotFound(new { mensaje = "Producto no encontrado" });

            return Ok(producto);
        }

        [HttpDelete("productos/{idProducto}")]
        public async Task<IActionResult> EliminarProducto(int idProducto)
        {
            try
            {
                var idProveedor = ObtenerIdProveedor();
                var resultado = await _productoService.EliminarProductoAsync(idProducto, idProveedor);

                if (!resultado)
                    return NotFound(new { mensaje = "Producto no encontrado o no pertenece al proveedor" });

                return NoContent(); // 204 No Content es lo estándar para DELETE exitoso
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar producto: {ex}");
                return StatusCode(500, new { mensaje = "Error interno al eliminar el producto" });
            }
        }


        [HttpPut("productos/{idProducto}")]
        public async Task<IActionResult> ActualizarProducto(int idProducto, [FromBody] UpdateProductoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var idProveedor = ObtenerIdProveedor();
                var producto = await _productoService.ActualizarProductoAsync(idProducto, dto, idProveedor);
                return Ok(producto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar producto: {ex}");
                return StatusCode(500, new { mensaje = "Error interno al actualizar el producto" });
            }
        }


    }
}