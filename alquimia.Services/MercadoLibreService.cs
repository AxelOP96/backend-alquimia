using alquimia.Data.Entities;
using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace alquimia.Services
{
    public class MercadoLibreService : IMercadoLibreService
    {
        private readonly IProductService _productService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly AlquimiaDbContext _context;
        private readonly MercadoLibreSettings _settings;

        public MercadoLibreService(
            IProductService productService,
            IHttpClientFactory httpClientFactory,
            AlquimiaDbContext context,
            Microsoft.Extensions.Options.IOptions<MercadoLibreSettings> settings)
        {
            _productService = productService;
            _httpClientFactory = httpClientFactory;
            _context = context;
            _settings = settings.Value;
        }

        public async Task ProcessOrderAsync(MercadoLibreOrderDTO dto)
        {
            await _productService.DecreaseVariantStockAsync(dto.VariantId, dto.Quantity);
        }

        public async Task SyncProductsAsync(int providerId, string accessToken)
        {
            using var client = _httpClientFactory.CreateClient();

            var userInfo = await client.GetFromJsonAsync<JsonElement>($"https://api.mercadolibre.com/users/me?access_token={accessToken}");
            var userId = userInfo.GetProperty("id").GetInt32();

            var search = await client.GetFromJsonAsync<JsonElement>($"https://api.mercadolibre.com/users/{userId}/items/search?access_token={accessToken}");
            var itemIds = search.GetProperty("results").EnumerateArray().Select(e => e.GetString()).Where(s => s != null).ToList();

            foreach (var id in itemIds)
            {
                if (id == null) continue;
                var item = await client.GetFromJsonAsync<JsonElement>($"https://api.mercadolibre.com/items/{id}?access_token={accessToken}");

                var title = item.GetProperty("title").GetString() ?? "";
                var price = item.GetProperty("price").GetDecimal();
                var stock = item.GetProperty("available_quantity").GetInt32();
                string? image = null;
                if (item.TryGetProperty("thumbnail", out var thumb))
                    image = thumb.GetString();

                var dto = new CreateProductoDTO
                {
                    Name = title.Length > 30 ? title.Substring(0, 30) : title,
                    Description = title,
                    TipoProductoDescription = "Mercado Libre",
                    Variants = new List<CreateProductVariantDTO>
                    {
                        new CreateProductVariantDTO
                        {
                            Volume = 0,
                            Unit = "u",
                            Price = price,
                            Stock = stock,
                            Image = image
                        }
                    }
                };

                try
                {
                    await EnsureProductTypeAsync("Mercado Libre");
                    await _productService.CreateProductAsync(dto, providerId);
                }
                catch
                {
                    // ignore individual item errors
                }
            }
        }

        public async Task SyncProductsFromCodeAsync(int providerId, string code, string redirectUri)
        {
            var token = await ExchangeCodeForTokenAsync(code, redirectUri);
            await SyncProductsAsync(providerId, token);
        }

        private async Task<string> ExchangeCodeForTokenAsync(string code, string redirectUri)
        {
            using var client = _httpClientFactory.CreateClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "client_id", _settings.AppId },
                { "client_secret", _settings.ClientSecret },
                { "code", code },
                { "redirect_uri", redirectUri }
            });

            var response = await client.PostAsync("https://api.mercadolibre.com/oauth/token", content);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadFromJsonAsync<JsonElement>();
            return json.GetProperty("access_token").GetString() ?? string.Empty;
        }

        private async Task EnsureProductTypeAsync(string description)
        {
            var exists = await _context.ProductTypes.AnyAsync(t => t.Description == description);
            if (!exists)
            {
                _context.ProductTypes.Add(new ProductType { Description = description });
                await _context.SaveChangesAsync();
            }
        }
    }
}
