using backendAlquimia.Models;

namespace backendAlquimia.Services.Interfaces
{
    public interface IFormulaService
    {
        Task<GETFormulaDTO> guardar(POSTFormulaDTO dto);
        Task<List<GETFormulaDTO>> ObtenerIntensidadAsync();
        Task<GETFormulaDTO> ObtenerPorId(int id);
    }
}
