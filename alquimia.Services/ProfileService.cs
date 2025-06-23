using alquimia.Data.Entities;
using alquimia.Services.Extensions;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly AlquimiaDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ProfileService(UserManager<User> userManager, AlquimiaDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserProfileDto?> BringMyData()
        {

            var user = await GetCurrentUserAsync();
            if (user == null) return null;

            return new UserProfileDto
            {
                Name = user.Name,
                Email = user.Email,
                EsProveedor = user.EsProveedor,
                //CUIL = user.CUIL,
                //Ubicacion = user.Ubicacion,
                //CodigoPostal = user.CodigoPostal
            };
        }

        private async Task<User?> GetCurrentUserAsync()
        {
            // --- 1. Obtener el Id del usuario autenticado ---------------------------
            var userIdString = _userManager.GetUserId(_httpContextAccessor.HttpContext?.User);
            if (!int.TryParse(userIdString, out var userId))
                return null;

            // --- 2. Traer el usuario con sus colecciones “ligeras” -------------------
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.Products)
                .Include(u => u.UserProducts)
                    .ThenInclude(up => up.Producto)
                .FirstOrDefaultAsync();

            if (user == null)
                return null;

            // --- 3. Cargar las fórmulas con TODOS los detalles -----------------------
            await _context.Entry(user)                         // apunta a la entidad en el ChangeTracker
                .Collection(u => u.Formulas)            // navega a la colección
                .Query()                                       // convierte a IQueryable<Formula>
                .IncludeFormulaNotesWithDetails()              // tu extensión con todos los Include
                .LoadAsync();                                  // ejecuta y llena la colección

            return user;
        }

        public async Task<List<Formula>> BringMyFormulasAsync()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<Formula>();

            return await _context.Formulas
                .Where(f => f.CreatorId == user.Id)
                .IncludeFormulaNotesWithDetails()
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<Product>> BringMyProducts()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<Product>();

            return user.Products.ToList();
        }

        public async Task<List<Product>> BringMyWishlist()
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
                return new List<Product>();

            return user.UserProducts.Select(up => up.Producto).ToList();
        }

        public async Task<UserProfileDto?> UpdateMyData(UserProfileDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return null;

            user.Name = dto.Name;
            //user.CUIL = dto.CUIL;
            //user.Ubicacion = dto.Ubicacion;
            //user.CodigoPostal = dto.CodigoPostal;

            await _context.SaveChangesAsync();

            return dto;
        }

        public Task<UserProfileDto?> UpdateProfileAsync(UpdateUserProfileDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
