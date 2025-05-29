//using alquimia.Data.Data;
//using backendAlquimia.Seed;
using alquimia.Data.Data.Entities;
using alquimia.Services.Services;
using alquimia.Services.Services.Interfaces;
using backendAlquimia.alquimia.Services;
using backendAlquimia.alquimia.Services.Interfaces;
using backendAlquimia.alquimia.Services.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
//using backendAlquimia.alquimia.Services.Services;
//using alquimia.Data.Data.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Alquimia API",
        Version = "v1",
    });
});

var connectionString = Environment.GetEnvironmentVariable("ALQUIMIA_DB_CONNECTION")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AlquimiaDbContext>(options =>
    options.UseSqlServer(connectionString));

var clientId = builder.Configuration["OAuth:ClientID"];
var clientSecret = builder.Configuration["OAuth:ClientSecret"];

// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<IQuizService, QuizService>();
//builder.Services.AddControllersWithViews().AddJsonOptions(options =>
//{
//    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Para que respete nombres C#
//});
builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles)
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

//builder.Services.AddIdentity<User, Role>()
//    .AddEntityFrameworkStores<AlquimiaDbContext>()
//.AddDefaultTokenProviders();

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
            "https://localhost:5035", //Swagger
            "https://localhost:5173"  // Vite auth
            )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});


var app = builder.Build();
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    //await RoleSeeder.SeedRolesAsync(services);
//    await UserSeeder.SeedAdminAsync(services);
//    await ProductoSeeder.SeedTiposProductoAsync(services);

//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alquimia API V1");
    c.RoutePrefix = string.Empty;
});

app.UseStaticFiles();
app.UseCors("FrontendPolicy");
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
