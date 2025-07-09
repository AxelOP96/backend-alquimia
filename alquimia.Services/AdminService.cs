using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Services
{
    public class AdminService : IAdminService
    {
        private readonly AlquimiaDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminService(AlquimiaDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<List<ProviderDTO>> GetPendingAndApprovedProvidersAsync()
        {
            var allUsers = await _userManager.Users
            .Where(u => u.EsProveedor)
            .AsNoTracking()
            .ToListAsync();
            var list = new List<ProviderDTO>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User: {user.Email}, EsProveedor: {user.EsProveedor}, Roles: {string.Join(", ", roles)}");
                if (roles.Contains("Creador") || roles.Contains("Proveedor"))
                {
                    list.Add(new ProviderDTO
                    {
                        Id = user.Id,
                        Nombre = user.Name,
                        Email = user.Email,
                        EsAprobado = roles.Contains("Proveedor")
                    });
                }
            }
            return list;
        }

        public async Task<ProviderDTO?> GetPendingOrApprovedProviderByIdAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.EsProveedor) return null;

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Creador") && !roles.Contains("Proveedor")) return null;

            return new ProviderDTO
            {
                Id = user.Id,
                Nombre = user.Name,
                Email = user.Email,
                EsAprobado = user.EsProveedor
            };
        }

        public async Task<bool> ApprovePendingProviderAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.EsProveedor) return false;

            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Creador"))
                await _userManager.RemoveFromRoleAsync(user, "Creador");

            if (!currentRoles.Contains("Proveedor"))
                await _userManager.AddToRoleAsync(user, "Proveedor");

            return true;
        }

        public async Task<bool> DeactivateProviderAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null || !user.EsProveedor) return false;

            user.EsProveedor = false;
            await _context.SaveChangesAsync();
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (currentRoles.Contains("Proveedor"))
                await _userManager.RemoveFromRoleAsync(user, "Proveedor");

            if (!currentRoles.Contains("Creador"))
                await _userManager.AddToRoleAsync(user, "Creador");
            return true;
        }

        public async Task<ProviderDTO?> GetPendingOrApprovedProviderByEmailAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null || !user.EsProveedor)
                throw new NullReferenceException();

            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Contains("Creador") && !roles.Contains("Proveedor"))
                throw new NullReferenceException();

            return new ProviderDTO
            {
                Id = user.Id,
                Nombre = user.Name,
                Email = user.Email,
                EsAprobado = user.EsProveedor
            };
        }
    }
}
