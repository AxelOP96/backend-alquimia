using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;

namespace alquimia.Services
{
    public class ProductService : IProductService
    {
        private readonly AlquimiaDbContext _context;
        private readonly IFormulaService _formulaService;

        public ProductService(AlquimiaDbContext context, IFormulaService formulaService)
        {
            _context = context;
            _formulaService = formulaService;
        }

        public async Task<List<ProductDTO>> GetProductsByProviderAsync(int idProveedor)
        {
            return await _context.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.TipoProducto)
                .Where(p => p.IdProveedor == idProveedor)
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    ProductType = p.TipoProducto.Description,
                    Provider = new ProviderDTO
                    {
                        Id = p.IdProveedorNavigation.Id,
                        Nombre = p.IdProveedorNavigation.Name,
                    },
                    Variants = p.ProductVariants.Select(v => new ProductVariantDTO
                    {
                        Id = v.Id,
                        Volume = v.Volume,
                        Unit = v.Unit,
                        Price = v.Price,
                        Stock = v.Stock,
                        IsHypoallergenic = v.IsHypoallergenic,
                        IsVegan = v.IsVegan,
                        IsParabenFree = v.IsParabenFree,
                        Image = v.Image
                    }).ToList()
                }).ToListAsync();
        }

        public async Task<ProductDTO> GetProductByIdAsync(int idProducto, int idProveedor)
        {
            var producto = await _context.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.TipoProducto)
                .FirstOrDefaultAsync(p => p.Id == idProducto && p.IdProveedor == idProveedor);

            if (producto == null)
                throw new KeyNotFoundException("Producto no encontrado");

            return new ProductDTO
            {
                Id = producto.Id,
                Name = producto.Name,
                Description = producto.Description,
                ProductType = producto.TipoProducto.Description,
                Variants = producto.ProductVariants.Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    Volume = v.Volume,
                    Unit = v.Unit,
                    Price = v.Price,
                    Stock = v.Stock,
                    IsHypoallergenic = v.IsHypoallergenic,
                    IsVegan = v.IsVegan,
                    IsParabenFree = v.IsParabenFree,
                    Image = v.Image
                }).ToList()
            };
        }

        public async Task<ProductDTO> CreateProductAsync(CreateProductoDTO dto, int idProveedor)
        {
            try
            {
                var tipoProducto = await _context.ProductTypes
                    .FirstOrDefaultAsync(t => t.Description == dto.TipoProductoDescription);

                if (tipoProducto == null)
                    throw new KeyNotFoundException("Tipo de producto no válido");

                var producto = new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    TipoProductoId = tipoProducto.Id,
                    IdProveedor = idProveedor,
                    ProductVariants = dto.Variants?.Select(v => new ProductVariant
                    {
                        Volume = v.Volume,
                        Unit = v.Unit,
                        Price = v.Price,
                        Stock = v.Stock,
                        IsHypoallergenic = v.IsHypoallergenic,
                        IsVegan = v.IsVegan,
                        IsParabenFree = v.IsParabenFree,
                        Image = v.Image
                    }).ToList()
                  ?? new List<ProductVariant>()
                };

                _context.Products.Add(producto);

                await _context.SaveChangesAsync();

                return await GetProductByIdAsync(producto.Id, idProveedor);
            }
            catch (DbUpdateException ex)
            {
                // Log de la excepción con inner exception
                var innerMessage = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine($"[Error al guardar producto] {innerMessage}");
                throw new Exception($"Error al guardar producto: {innerMessage}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error general] {ex.Message}");
                throw;
            }
        }

        public async Task<bool> DeleteProductAsync(int idProducto, int idProveedor)
        {
            var producto = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == idProducto && p.IdProveedor == idProveedor);

            if (producto == null)
                return false;

            // Eliminar variantes primero
            _context.ProductVariants.RemoveRange(producto.ProductVariants);

            // Luego eliminar el producto
            _context.Products.Remove(producto);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<ProductDTO> UpdateProductAsync(int idProducto, UpdateProductoDTO dto, int idProveedor)
        {
            var producto = await _context.Products
                .Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == idProducto && p.IdProveedor == idProveedor);

            if (producto == null)
                throw new KeyNotFoundException("Producto no encontrado o no pertenece al proveedor");

            if (!string.IsNullOrWhiteSpace(dto.Name))
                producto.Name = dto.Name;

            if (!string.IsNullOrWhiteSpace(dto.Description))
                producto.Description = dto.Description;

            await _context.SaveChangesAsync();
            return await GetProductByIdAsync(idProducto, idProveedor);
        }
        public async Task<HomeProviderDataDTO> GetHomeDataAsync(int idProveedor)
        {
            var productos = await GetProductsByProviderAsync(idProveedor);

            return new HomeProviderDataDTO
            {
                TotalProductos = productos.Count,
                StockTotal = productos.SelectMany(p => p.Variants).Sum(v => v.Stock),
                UltimosProductos = productos.OrderByDescending(p => p.Id).Take(5).ToList()
            };
        }

        public async Task<PriceRangeDTO> GetPriceRangeFromProductAsync(int noteId)
        {
            var note = await _context.Notes.FindAsync(noteId);

            var query = _context.ProductVariants
                .Where(v =>
                    v.Product.Name.ToLower().Contains(note.Name.ToLower()) ||
                    v.Product.Description.ToLower().Contains(note.Name.ToLower()) ||
                    v.Product.Name.ToLower().Contains(note.Description.ToLower()) ||
                    v.Product.Description.ToLower().Contains(note.Description.ToLower())
                );

            var prices = await query.Select(v => v.Price).ToListAsync();
            if (!prices.Any())
                return new PriceRangeDTO { MinPrice = 0, MaxPrice = 0 };

            return new PriceRangeDTO
            {
                MinPrice = await query.MinAsync(v => (decimal?)v.Price) ?? 0,
                MaxPrice = await query.MaxAsync(v => (decimal?)v.Price) ?? 0
            };
        }
        public async Task<ProductDTO> GetProductByIdAsync(int idProducto)
        {
            var producto = await _context.Products
                .Include(p => p.ProductVariants)
                .Include(p => p.TipoProducto)
                .Include(p => p.IdProveedorNavigation)
                .FirstOrDefaultAsync(p => p.Id == idProducto);

            if (producto == null)
                throw new KeyNotFoundException("Producto no encontrado");

            return new ProductDTO
            {
                Id = producto.Id,
                Name = producto.Name,
                Description = producto.Description,
                ProductType = producto.TipoProducto.Description,
                Provider = new ProviderDTO
                {
                    Id = producto.IdProveedorNavigation.Id,
                    Nombre = producto.IdProveedorNavigation.Name,
                },
                Variants = producto.ProductVariants.Select(v => new ProductVariantDTO
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
            };
        }
        public async Task AddVariantsToProductAsync(int productId, CreateProductVariantDTO dto)
        {
            var producto = await _context.Products.Include(p => p.ProductVariants)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (producto == null)
                throw new KeyNotFoundException("Producto no encontrado");

            var variante = new ProductVariant
            {
                Volume = dto.Volume,
                Unit = dto.Unit,
                Price = dto.Price,
                Stock = dto.Stock,
                IsHypoallergenic = dto.IsHypoallergenic,
                IsVegan = dto.IsVegan,
                IsParabenFree = dto.IsParabenFree,
                Image = dto.Image,
                ProductId = productId
            };

            _context.ProductVariants.Add(variante);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVariantAsync(int variantId, UpdateProductVariantDTO dto)
        {
            var variante = await _context.ProductVariants.FindAsync(variantId);
            if (variante == null)
                throw new Exception("Variante no encontrada");

            if (dto.Volume.HasValue)
                variante.Volume = dto.Volume.Value;

            if (!string.IsNullOrEmpty(dto.Unit))
                variante.Unit = dto.Unit;

            if (dto.Price.HasValue)
                variante.Price = dto.Price.Value;

            if (dto.Stock.HasValue)
                variante.Stock = dto.Stock.Value;

            if (dto.IsParabenFree.HasValue)
                variante.IsParabenFree = dto.IsParabenFree.Value;

            if (dto.IsHypoallergenic.HasValue)
                variante.IsHypoallergenic = dto.IsHypoallergenic.Value;

            if (dto.IsVegan.HasValue)
                variante.IsVegan = dto.IsVegan.Value;

            if (!string.IsNullOrEmpty(dto.Image))
                variante.Image = dto.Image;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUpdatedVariantAsync(int variantId, ProductVariantDTO dto)
        {
            var variante = await _context.ProductVariants.FindAsync(variantId);
            if (variante == null) return false;

            variante.Volume = dto.Volume;
            variante.Unit = dto.Unit;
            variante.Price = dto.Price;
            variante.Stock = dto.Stock;
            variante.IsHypoallergenic = dto.IsHypoallergenic;
            variante.IsVegan = dto.IsVegan;
            variante.IsParabenFree = dto.IsParabenFree;
            variante.Image = dto.Image;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteVariantAsync(int variantId)
        {
            var variante = await _context.ProductVariants.FindAsync(variantId);
            if (variante == null)
                return false;

            _context.ProductVariants.Remove(variante);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<ProductDTO>> GetProductsByFormulaAsync(int formulaId)
        {
            var formula = await _context.Formulas
        .Include(f => f.FormulaSalidaNavigation)
            .ThenInclude(fn => fn.NotaId1Navigation)
        .Include(f => f.FormulaSalidaNavigation.NotaId2Navigation)
        .Include(f => f.FormulaSalidaNavigation.NotaId3Navigation)
        .Include(f => f.FormulaSalidaNavigation.NotaId4Navigation)
        .Include(f => f.FormulaCorazonNavigation)
            .ThenInclude(fn => fn.NotaId1Navigation)
        .Include(f => f.FormulaCorazonNavigation.NotaId2Navigation)
        .Include(f => f.FormulaCorazonNavigation.NotaId3Navigation)
        .Include(f => f.FormulaCorazonNavigation.NotaId4Navigation)
        .Include(f => f.FormulaFondoNavigation)
            .ThenInclude(fn => fn.NotaId1Navigation)
        .Include(f => f.FormulaFondoNavigation.NotaId2Navigation)
        .Include(f => f.FormulaFondoNavigation.NotaId3Navigation)
        .Include(f => f.FormulaFondoNavigation.NotaId4Navigation)
        .FirstOrDefaultAsync(f => f.Id == formulaId);

            if (formula == null)
                throw new KeyNotFoundException();

            var noteNames = new List<string?>
            {
                formula.FormulaSalidaNavigation.NotaId1Navigation?.Name,
                formula.FormulaSalidaNavigation.NotaId2Navigation?.Name,
                formula.FormulaSalidaNavigation.NotaId3Navigation?.Name,
                formula.FormulaSalidaNavigation.NotaId4Navigation?.Name,

                formula.FormulaCorazonNavigation.NotaId1Navigation?.Name,
                formula.FormulaCorazonNavigation.NotaId2Navigation?.Name,
                formula.FormulaCorazonNavigation.NotaId3Navigation?.Name,
                formula.FormulaCorazonNavigation.NotaId4Navigation?.Name,

                formula.FormulaFondoNavigation.NotaId1Navigation?.Name,
                formula.FormulaFondoNavigation.NotaId2Navigation?.Name,
                formula.FormulaFondoNavigation.NotaId3Navigation?.Name,
                formula.FormulaFondoNavigation.NotaId4Navigation?.Name,
            }
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Distinct()
            .ToList();

            noteNames = noteNames.Select(n => n!.ToLower()).ToList();

            var productos = await _context.Products
                .Include(p => p.TipoProducto)
                .Include(p => p.IdProveedorNavigation)
                .Include(p => p.ProductVariants)
                .Where(p =>
                    noteNames.Any(n =>
                        p.Name.ToLower().Contains(n!.ToLower()) || p.Description.ToLower().Contains(n!.ToLower())
                    )
                )
                .ToListAsync();

            return productos.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.TipoProducto.Description,
                Provider = new ProviderDTO
                {
                    Id = p.IdProveedorNavigation.Id,
                    Nombre = p.IdProveedorNavigation.Name,
                },
                Variants = p.ProductVariants.Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    Image = v.Image,
                    Volume = v.Volume,
                    Unit = v.Unit,
                    Price = v.Price,
                    Stock = v.Stock,
                    IsHypoallergenic = v.IsHypoallergenic,
                    IsVegan = v.IsVegan,
                    IsParabenFree = v.IsParabenFree
                }).ToList()
            }).ToList();
        }
        public async Task<List<ProductDTO>> GetAllAsync()
        {
            var productos = await _context.Products
                .Include(p => p.TipoProducto)
                .Include(p => p.IdProveedorNavigation)
                .Include(p => p.ProductVariants)
                .ToListAsync();

            if (productos == null)
            {
                throw new KeyNotFoundException();
            }

            return productos.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.TipoProducto.Description,
                Provider = new ProviderDTO
                {
                    Id = p.IdProveedorNavigation.Id,
                    Nombre = p.IdProveedorNavigation.Name,
                    Email = p.IdProveedorNavigation.Email
                },
                Variants = p.ProductVariants
                    .Where(v => v.Price > 0)
                    .Select(v => new ProductVariantDTO
                    {
                        Id = v.Id,
                        Image = v.Image,
                        Volume = v.Volume,
                        Unit = v.Unit,
                        Price = v.Price,
                        Stock = v.Stock,
                        IsHypoallergenic = v.IsHypoallergenic,
                        IsVegan = v.IsVegan,
                        IsParabenFree = v.IsParabenFree
                    }).ToList(),

                Price = p.ProductVariants
                    .Where(v => v.Price > 0)
                    .OrderBy(v => v.Price)
                    .Select(v => v.Price)
                    .FirstOrDefault(),

                Volume = p.ProductVariants
                    .Where(v => v.Price > 0)
                    .OrderBy(v => v.Price)
                    .Select(v => (int?)v.Volume)
                    .FirstOrDefault(),

                Unit = p.ProductVariants
                    .Where(v => v.Price > 0)
                    .OrderBy(v => v.Price)
                    .Select(v => v.Unit)
                    .FirstOrDefault()
            }).ToList();
        }


        public async Task<List<ProductDTO>> GetAllAlcoholsAsync()
        {
            var productos = await _context.Products
                .Include(p => p.TipoProducto)
                .Include(p => p.IdProveedorNavigation)
                .Include(p => p.ProductVariants)
                .Where(p => p.TipoProducto.Description.ToLower() == "alcohol")
                .ToListAsync();

            if (productos == null || productos.Count() == 0)
            {
                throw new NullReferenceException();
            }
            return productos.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.TipoProducto.Description,
                Provider = new ProviderDTO
                {
                    Id = p.IdProveedorNavigation.Id,
                    Nombre = p.IdProveedorNavigation.Name,
                },
                Variants = p.ProductVariants
                .Where(v => v.Price > 0)
                .Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    Image = v.Image,
                    Volume = v.Volume,
                    Unit = v.Unit,
                    Price = v.Price,
                    Stock = v.Stock,
                    IsHypoallergenic = v.IsHypoallergenic,
                    IsVegan = v.IsVegan,
                    IsParabenFree = v.IsParabenFree
                }).ToList(),

                Price = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => v.Price)
                .FirstOrDefault(),

                Volume = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => (int?)v.Volume)
                .FirstOrDefault(),

                Unit = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => v.Unit)
                .FirstOrDefault()

            }).ToList();
        }
        public async Task<ProductVariant> GetVariantEntityAsync(int variantId)
        {
            var variant = await _context.ProductVariants
                .Include(v => v.Product)
                .FirstOrDefaultAsync(v => v.Id == variantId);

            if (variant == null)
                throw new KeyNotFoundException("Variante no encontrada");

            return variant;
        }

        public async Task DecreaseVariantStockAsync(int variantId, int quantity)
        {
            if (quantity < 0)
                throw new ArgumentException("Cantidad inválida", nameof(quantity));

            var variant = await _context.ProductVariants.FindAsync(variantId);
            if (variant == null)
                throw new KeyNotFoundException("Variante no encontrada");

            if (variant.Stock < quantity)
                throw new InvalidOperationException("Stock insuficiente");

            variant.Stock -= quantity;
            await _context.SaveChangesAsync();
        }
        public async Task<List<ProductDTO>> GetAllBottlesAsync()
        {
            var productos = await _context.Products
                .Include(p => p.TipoProducto)
                .Include(p => p.IdProveedorNavigation)
                .Include(p => p.ProductVariants)
                .Where(p => p.TipoProducto.Description.ToLower() == "envase")
                .ToListAsync();

            if (productos == null || productos.Count() == 0)
            {
                throw new NullReferenceException();
            }

            return productos.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProductType = p.TipoProducto.Description,
                Provider = new ProviderDTO
                {
                    Id = p.IdProveedorNavigation.Id,
                    Nombre = p.IdProveedorNavigation.Name,
                },
                Variants = p.ProductVariants
                .Where(v => v.Price > 0)
                .Select(v => new ProductVariantDTO
                {
                    Id = v.Id,
                    Image = v.Image,
                    Volume = v.Volume,
                    Unit = v.Unit,
                    Price = v.Price,
                    Stock = v.Stock,
                    IsHypoallergenic = v.IsHypoallergenic,
                    IsVegan = v.IsVegan,
                    IsParabenFree = v.IsParabenFree
                }).ToList(),

                Price = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => v.Price)
                .FirstOrDefault(),

                Volume = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => (int?)v.Volume)
                .FirstOrDefault(),

                Unit = p.ProductVariants
                .Where(v => v.Price > 0)
                .OrderBy(v => v.Price)
                .Select(v => v.Unit)
                .FirstOrDefault()

            }).ToList();
        }
        public async Task AddToWishlistAsync(int userId, int productId)
        {
            var exists = await _context.UserProducts
                .AnyAsync(up => up.UsuarioId == userId && up.ProductoId == productId);

            if (!exists)
            {
                var userProduct = new UserProduct
                {
                    UsuarioId = userId,
                    ProductoId = productId
                };

                _context.UserProducts.Add(userProduct);
                await _context.SaveChangesAsync();
            }
        }
        public async Task RemoveFromWishlistAsync(int userId, int productId)
        {
            var userProduct = await _context.UserProducts
                .FirstOrDefaultAsync(up => up.UsuarioId == userId && up.ProductoId == productId);

            if (userProduct != null)
            {
                _context.UserProducts.Remove(userProduct);
                await _context.SaveChangesAsync();
            }
        }


    }
}
