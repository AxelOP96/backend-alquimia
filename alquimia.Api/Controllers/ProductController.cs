using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
namespace alquimia.Api.Controllers
{
    //[Authorize]
    [Route("product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productoservice)
        {
            _productService = productoservice;
        }

        [HttpGet("price-range/{noteId}")]
        public async Task<IActionResult> GetPriceRange(int noteId)
        {
            var priceRange = await _productService.GetPriceRangeFromProductAsync(noteId);
            return Ok(priceRange);
        }

        [HttpPost("get-products-by-formula")]
        public async Task<IActionResult> GetProductsByFormula([FromBody] SearchByFormulaDTO dto)
        {
            var products = await _productService.GetProductsByFormulaAsync(dto.FormulaId);
            return Ok(products);
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductoDTO dto)
        {
            // Simular proveedor autenticado
            var idProveedor = 1; // TODO: reemplazar por el id real del proveedor autenticado

            try
            {
                var product = await _productService.CreateProductAsync(dto, idProveedor);
                return Ok(product);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                return Ok(product);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Producto con ID {id} no encontrado." });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetProductsByFormula()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("alcohols")]
        public async Task<IActionResult> GetAllAlcoholsAsync()
        {
            var products = await _productService.GetAllAlcoholsAsync();
            return Ok(products);
        }

        [HttpGet("bottles")]
        public async Task<IActionResult> GetAllBottlesAsync()
        {
            var products = await _productService.GetAllBottlesAsync();
            return Ok(products);
        }
        public class AddToWishlistDTO
        {
            public int ProductId { get; set; }
        }
        [HttpPost("wishlist-add")]
        [Authorize]
        public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistDTO dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized("No se pudo obtener el ID del usuario");

            try
            {
                await _productService.AddToWishlistAsync(userId, dto.ProductId);
                return Ok(new { message = "Producto agregado a tu biblioteca" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("wishlist-remove/{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            await _productService.RemoveFromWishlistAsync(userId, productId);
            return Ok(new { message = "Producto eliminado de tu biblioteca." });
        }


    }
}
