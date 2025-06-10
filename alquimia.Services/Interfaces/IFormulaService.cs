using alquimia.Data.Entities;
using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IFormulaService
    {
        Task<GETFormulaDTO> GetFormulaByIdToDTOAsync(int id);
        Task<int> SaveAsync(POSTFormulaDTO formula);
        Task<List<IntensityDTO>> GetIntensitiesAsync();
        Task UpdateTitleAsync(Formula? found, string title);
        Task<Formula?> GetFormulaAsync(int id);
        Task<List<GETFormulaDTO?>> GetFormulasByCreatorId(int id);
    }
}
