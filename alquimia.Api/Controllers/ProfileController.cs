using System.Security.Claims;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace alquimia.Api.Controllers
{
    [Route("profile")]
    [ApiController]

    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;

        }

        [HttpGet("data")]
        public async Task<IActionResult> GetMyData()
        {
            var data = await _profileService.BringMyData();
            if (data == null)
                return NotFound("Usuario no encontrado");

            return Ok(data);
        }

        [HttpGet("formulas")]
        public async Task<IActionResult> GetMyFormulas()
        {
            var formulas = await _profileService.BringMyFormulasAsync();
            return Ok(formulas);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetMyProducts()
        {
            var products = await _profileService.BringMyProducts();
            return Ok(products);
        }

        [HttpGet("wishlist")]
        public async Task<IActionResult> GetWishlist()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlist = await _profileService.GetUserWishlistAsync(userId);
            return Ok(wishlist);
        }

        [HttpPut("data")]
        public async Task<IActionResult> UpdateMyData([FromBody] UserProfileUpdateDto updatedData)
        {
            var user = await _profileService.UpdateMyData(updatedData);
            if (user == null)
                return NotFound("Usuario no encontrado");

            return Ok(user);
        }
    }
}
