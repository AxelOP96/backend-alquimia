using alquimia.Data.Entities;
using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IProfileService
    {
        Task<List<Product>> BringMyProducts();
        Task<List<Formula>> BringMyFormulasAsync();
        Task<UserProfileDto?> BringMyData();
        Task<List<ProductDTO>> GetUserWishlistAsync(string userId);
        Task<UserProfileDto?> UpdateMyData(UserProfileUpdateDto updatedData);
    }
}
