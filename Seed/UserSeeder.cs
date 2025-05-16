using backendAlquimia.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace backendAlquimia.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<Usuario>>();
            var email = "admin@alquimia.com";
            var password = "Admin123!"; 

            var existingUser = await userManager.FindByEmailAsync(email);
            if (existingUser == null)
            {
                var admin = new Usuario
                {
                    UserName = email,
                    Email = email,
                    Name = "Administrador"
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
