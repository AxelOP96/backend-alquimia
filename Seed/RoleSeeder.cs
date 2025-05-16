using backendAlquimia.Data.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

namespace backendAlquimia.Seed
{
    public class RoleSeeder
    {
        private static readonly string[] Roles = { "Admin", "Creador", "Proveedor" };

        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<Rol>>();

            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Rol { Name = role });
                }
            }
        }
    }
}
