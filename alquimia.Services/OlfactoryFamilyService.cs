using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Services
{
    public class OlfactoryFamilyService : IOlfactoryFamilyService
    {
        private readonly AlquimiaDbContext _context;

        public OlfactoryFamilyService(AlquimiaDbContext context)
        {
            _context = context;
        }
        public async Task<OlfactoryFamilyDTO> GetOlfactoryFamilyInfoAsync(int id)
        {
            var found = await _context.OlfactoryFamilies.FirstOrDefaultAsync(f => f.Id == id);
            if (found == null)
            {
                throw new KeyNotFoundException();
            }
            return new OlfactoryFamilyDTO
            {
                Id = found.Id,
                Name = found.Nombre,
                Description = found.Description,
                Image1 = found.Image1,
            };
        }
        public async Task<int> CreateOlfactoryFamilyAsync(OlfactoryFamilyDTO familyDTO)
        {
            var family = new OlfactoryFamily
            {
                Nombre = familyDTO.Name,
                Description = familyDTO.Description,
                Image1 = familyDTO.Image1
            };

            _context.OlfactoryFamilies.Add(family);
            await _context.SaveChangesAsync();

            return family.Id;
        }
        public async Task UpdateOlfactoryFamilyAsync(OlfactoryFamily family)
        {
            _context.OlfactoryFamilies.Update(family);
            await _context.SaveChangesAsync();
        }
        public async Task<List<OlfactoryFamilyDTO>> GetAllFamilies()
        {
            var families = await _context.OlfactoryFamilies.ToListAsync();

            return families.Select(f => new OlfactoryFamilyDTO
            {
                Id = f.Id,
                Name = f.Nombre,
                Description = f.Description,
            }).ToList();
        }

    }
}
