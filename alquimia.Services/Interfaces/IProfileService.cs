using alquimia.Data.Entities;
using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IProfileService
    {
        Task<List<Product>> BringMyProducts();
        Task<List<Formula>> BringMyFormulasAsync();
        Task<UserProfileDto?> BringMyData();
        Task<List<Product>> BringMyWishlist();
        Task<UserProfileDto?> UpdateProfileAsync(UpdateUserProfileDto dto);
        Task<UserProfileDto?> UpdateMyData(UserProfileDto updatedData);
    }
}
