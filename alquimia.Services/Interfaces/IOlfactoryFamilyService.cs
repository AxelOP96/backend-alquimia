using alquimia.Services.Models;

namespace alquimia.Services.Interfaces
{
    public interface IOlfactoryFamilyService
    {
        Task<OlfactoryFamilyDTO> GetOlfactoryFamilyInfoAsync(int id);
        Task<List<OlfactoryFamilyDTO>> GetAllFamilies();
    }
}
