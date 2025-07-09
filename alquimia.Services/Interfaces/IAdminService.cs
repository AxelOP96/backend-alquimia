using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IAdminService
    {
        Task<List<ProviderDTO>> GetPendingAndApprovedProvidersAsync();
        Task<ProviderDTO?> GetPendingOrApprovedProviderByIdAsync(int id);
        Task<bool> ApprovePendingProviderAsync(int id);
        Task<bool> DeactivateProviderAsync(int id);
        Task<ProviderDTO?> GetPendingOrApprovedProviderByEmailAsync(string email);
    }
}
