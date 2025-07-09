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
                IdEstado = user.IdEstado,
                CUIL = user.Cuil,
                cantidadFavoritos = user.UserProducts.Count,
                cantidadFormulas = user.Formulas.Count,
                //Ubicacion = user.Ubicacion,
                //CodigoPostal = user.CodigoPostal
                Empresa = user.Empresa,
                Rubro = user.Rubro,
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

        public async Task<List<ProductDTO>> GetUserWishlistAsync(string userId)
        {
            var userProducts = await _context.UserProducts
            .Include(up => up.Producto)
                .ThenInclude(p => p.ProductVariants)
            .Where(up => up.UsuarioId.ToString() == userId && up.Producto != null)
            .Select(up => up.Producto)
            .ToListAsync();

            return userProducts.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Variants = p.ProductVariants.Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    Volume = v.Volume,
                    Unit = v.Unit,
                    Price = v.Price,
                    Stock = v.Stock,
                    Image = v.Image,
                    IsHypoallergenic = v.IsHypoallergenic,
                    IsVegan = v.IsVegan,
                    IsParabenFree = v.IsParabenFree
                }).ToList()
            }).ToList();
        }


        public async Task<UserProfileDto?> UpdateMyData(UserProfileUpdateDto dto)
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return null;

            if (dto.Name != null)
                user.Name = dto.Name;
            if (dto.Empresa != null)
                user.Empresa = dto.Empresa;
            if (dto.CUIL != null)
                user.Cuil = dto.CUIL;
            if (dto.Rubro != null)
                user.Rubro = dto.Rubro;
            //user.Ubicacion = dto.Ubicacion;
            //user.CodigoPostal = dto.CodigoPostal;

            // Persist the changes using the identity user manager to
            // ensure all related identity fields are handled correctly
            _context.Users.Update(user);
            await _context.SaveChangesAsync();


            return new UserProfileDto
            {
                Name = user.Name,
                Email = user.Email,
                EsProveedor = user.EsProveedor,
                IdEstado = user.IdEstado,
                CUIL = user.Cuil,
                Empresa = user.Empresa,
                Rubro = user.Rubro,
                cantidadFavoritos = user.UserProducts.Count,
                cantidadFormulas = user.Formulas.Count,
            };
        }
    }
}