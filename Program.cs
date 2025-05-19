using backendAlquimia.Data;
using backendAlquimia.Data.Entities;
using backendAlquimia.Seed;
using backendAlquimia.Services;
using backendAlquimia.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = Environment.GetEnvironmentVariable("ALQUIMIA_DB_CONNECTION")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

var clientId = builder.Configuration["OAuth:ClientID"];
var clientSecret = builder.Configuration["OAuth:ClientSecret"];

// Add services to the container.
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<INotaService, NotaService>();
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddControllersWithViews().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Para que respete nombres C#
});
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<AlquimiaDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<AlquimiaDbContext>()
    .AddDefaultTokenProviders();
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None; // importante
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // asegura HTTPS
});



builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:ClientID"];
        options.ClientSecret = builder.Configuration["OAuth:ClientSecret"];
        options.CallbackPath = "/signin-google";
        options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    });




builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:3000",// Next.js dev server
            "https://localhost:5173"  // Vite auth
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAsync(services);
    await UserSeeder.SeedAdminAsync(services);

}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("FrontendPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
