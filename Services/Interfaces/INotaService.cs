using backendAlquimia.Models;

namespace backendAlquimia.Services.Interfaces
{
    public interface INotaService
    {
        Task<List<NotasPorFamiliaDTO>> ObtenerNotasDeSalidaAgrupadasPorFamiliaAsync();
        Task<List<NotasPorFamiliaDTO>> ObtenerNotasDeCorazonAgrupadasPorFamiliaAsync();
        Task<List<NotasPorFamiliaDTO>> ObtenerNotasDeFondoAgrupadasPorFamiliaAsync();
    }
}