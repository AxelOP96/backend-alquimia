using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IMercadoLibreService
    {
        Task ProcessOrderAsync(MercadoLibreOrderDTO dto);
        Task SyncProductsAsync(int providerId, string accessToken);
    }
}
