using backendAlquimia.Data;
using backendAlquimia.Data.Entities;
using backendAlquimia.Services;
using backendAlquimia.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// ► CONFIGURACIONES BÁSICAS
// ─────────────────────────────────────────────
var connectionString = Environment.GetEnvironmentVariable("ALQUIMIA_DB_CONNECTION")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

// OAuth (Google): se leerán sólo si existen
var googleId = builder.Configuration["OAuth:ClientID"];
var googleSecret = builder.Configuration["OAuth:ClientSecret"];

// ─────────────────────────────────────────────
// ► SERVICIOS
// ─────────────────────────────────────────────
builder.Services.AddControllersWithViews()
                .AddJsonOptions(o => o.JsonSerializerOptions.PropertyNamingPolicy = null);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<INotaService, NotaService>();

builder.Services.AddDbContext<AlquimiaDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddIdentity<Usuario, Rol>()
    .AddEntityFrameworkStores<AlquimiaDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opt =>
{
    opt.Cookie.SameSite = SameSiteMode.None;
    opt.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Autenticación base
builder.Services.AddAuthentication();

// Google OAuth solo si existen credenciales válidas
if (!string.IsNullOrWhiteSpace(googleId) && !string.IsNullOrWhiteSpace(googleSecret))
{
    builder.Services.AddAuthentication()
        .AddGoogle(opt =>
        {
            opt.ClientId = googleId;
            opt.ClientSecret = googleSecret;
            opt.CallbackPath = "/signin-google";
            opt.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
        });
}

// CORS para el front‑end
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:3000", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// ─────────────────────────────────────────────
// ► PIPELINE HTTP
// ─────────────────────────────────────────────

if (app.Environment.IsDevelopment())
{
    // Swagger habilitado solo en desarrollo
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
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
