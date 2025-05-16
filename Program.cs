using backendAlquimia.Data;
using backendAlquimia.Data.Entities;
using backendAlquimia.Services;
using backendAlquimia.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ─────────────────────────────────────────────
// ► CONFIGURACIONES BÁSICAS
// ─────────────────────────────────────────────
var connectionString = Environment.GetEnvironmentVariable("ALQUIMIA_DB_CONNECTION")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

// OAuth de Google (opcional)
var googleId = builder.Configuration["OAuth:ClientID"];
var googleSecret = builder.Configuration["OAuth:ClientSecret"];

// ─────────────────────────────────────────────
// ► REGISTRO DE SERVICIOS
// ─────────────────────────────────────────────
builder.Services.AddControllersWithViews()
       .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<INotaService, NotaService>();

builder.Services.AddDbContext<AlquimiaDbContext>(opt =>
    opt.UseSqlServer(connectionString));

builder.Services.AddIdentity<Usuario, Rol>()
       .AddEntityFrameworkStores<AlquimiaDbContext>()
       .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(opts =>
{
    opts.Cookie.SameSite = SameSiteMode.None;
    opts.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
       .AddCookie();

if (!string.IsNullOrWhiteSpace(googleId) && !string.IsNullOrWhiteSpace(googleSecret))
{
    builder.Services.AddAuthentication()
       .AddGoogle(opts =>
       {
           opts.ClientId = googleId;
           opts.ClientSecret = googleSecret;
           opts.CallbackPath = "/signin-google";
           opts.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
       });
}

builder.Services.AddCors(opts =>
{
    opts.AddPolicy("FrontendPolicy", policy =>
        policy.WithOrigins("http://localhost:3000", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

var app = builder.Build();

// ─────────────────────────────────────────────
// ► BORRAR Y CREAR LA BD SEGÚN EL MODELO
//   (ejecuta también todos los HasData() definidos
//    dentro de AlquimiaDbContext.OnModelCreating)
// ─────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AlquimiaDbContext>();
    db.Database.EnsureDeleted();
    db.Database.EnsureCreated();
}

// ─────────────────────────────────────────────
// ► PIPELINE HTTP
// ─────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
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
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();