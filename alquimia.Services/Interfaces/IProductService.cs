using alquimia.Data.Entities;
using alquimia.Services.Models;
namespace alquimia.Services.Interfaces
{
    public interface IProductService
    {
        Task<List<ProductDTO>> GetProductsByProviderAsync(int idProveedor);
        Task<ProductDTO> GetProductByIdAsync(int idProducto, int idProveedor);
        Task<ProductDTO> CreateProductAsync(CreateProductoDTO dto, int idProveedor);
        Task<bool> DeleteProductAsync(int idProducto, int idProveedor);
        Task<ProductDTO> UpdateProductAsync(int idProducto, UpdateProductoDTO dto, int idProveedor);
        Task<HomeProviderDataDTO> GetHomeDataAsync(int idProveedor);
        Task<PriceRangeDTO> GetPriceRangeFromProductAsync(int noteId);
        Task AddVariantsToProductAsync(int productId, CreateProductVariantDTO dto);
        Task UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto);
        Task<bool> DeleteVariantAsync(int variantId);
        Task<bool> IsUpdatedVariantAsync(int variantId, ProductVariantDTO dto);
        Task<List<ProductDTO>> GetProductsByFormulaAsync(int formulaId);
        Task<List<ProductDTO>> GetAllAsync();
        Task<ProductDTO> GetProductByIdAsync(int idProducto);
        Task<List<ProductDTO>> GetAllAlcoholsAsync();
        Task<List<ProductDTO>> GetAllBottlesAsync();
        Task<ProductVariant> GetVariantEntityAsync(int variantId);
        Task DecreaseVariantStockAsync(int variantId, int quantity);
        Task AddToWishlistAsync(int productId, int userId);
        Task RemoveFromWishlistAsync(int userId, int productId);
    }
}
