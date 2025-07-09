// Program.cs – Alquimia API (corrigido)
using alquimia.Services.Models;
using Microsoft.AspNetCore.DataProtection; 
using alquimia.Api.Middlewares;
using alquimia.Api.Seed;
using alquimia.Data.Entities;
using alquimia.Services;
using alquimia.Services.Handler;
using alquimia.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Azure.Identity; // Requiere Azure.Identity package
using Azure.Extensions.AspNetCore.DataProtection.Blobs; // Requiere Azure.Extensions.AspNetCore.DataProtection.Blobs

var builder = WebApplication.CreateBuilder(args);

// 🔗 Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AlquimiaDbContext>(options =>
{
    options.UseSqlServer(connectionString);

    // Solo para Development: registra datos sensibles en los logs
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
    }
});

// 💡 Servicios y dependencias
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
builder.Services.Configure<MercadoLibreSettings>(builder.Configuration.GetSection("MercadoLibre"));
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<IQuizService, QuizService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IProfileService, ProfileService>();
builder.Services.AddScoped<IOlfactoryFamilyService, OlfactoryFamilyService>();
builder.Services.AddScoped<IDesignLabelService, DesignLabelService>();
builder.Services.AddScoped<IEmailTemplateService, EmailTemplateService>(); // 🔄 Registro único
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IMercadoPagoService, MercadoPagoService>();
builder.Services.AddScoped<IMercadoLibreService, MercadoLibreService>();
builder.Services.AddScoped<IChatbotService, ChatbotService>();

// Handlers dinámicos
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicNotesHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicTopNotesHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicHeartNotesHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicBaseNotesHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicFamilyHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicIntensitiesHandler>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicStateProviderHelpResponse>();
builder.Services.AddScoped<IChatDynamicNodeHandler, DinamicStateProviderHelp>();

// 🧩 Controladores
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// 🔐 Identity
builder.Services.AddIdentity<User, Role>()
    .AddEntityFrameworkStores<AlquimiaDbContext>()
    .AddDefaultTokenProviders();

// 🔐 JWT & Google – un solo AddAuthentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
// JWT Bearer
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
})
// Google OAuth
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["OAuth:ClientID"];
    options.ClientSecret = builder.Configuration["OAuth:ClientSecret"];
    options.CallbackPath = "/account/signin-google";
    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    options.CorrelationCookie.SameSite = SameSiteMode.None;
    options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 🔑 Data Protection – persistencia de claves para reset de contraseña
if (!builder.Environment.IsDevelopment())
{
    var blobUri = builder.Configuration["DataProtection:BlobUri"]; // p.ej.: https://<storage>.blob.core.windows.net/dpkeys/keys.xml
    if (!string.IsNullOrWhiteSpace(blobUri))
    {
        builder.Services.AddDataProtection()
            .SetApplicationName("AlquimiaAPI")
            .PersistKeysToAzureBlobStorage(new Uri(blobUri), new DefaultAzureCredential());
    }
}
else
{
    builder.Services.AddDataProtection().SetApplicationName("AlquimiaAPI");
}

// 🍪 Cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// 📃 Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Alquimia API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header usando el esquema Bearer.  \nIngresá el token así: Bearer {tu token}.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

// 🔓 CORS incluyendo Vercel
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",              // Next.js dev
            "https://localhost:5035",             // Swagger
            "https://localhost:5173",             // Vite dev
            "https://frontend-alquimia.vercel.app" // Producción
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// ⚙️ Configuración de Identity extra
builder.Services.Configure<IdentityOptions>(options =>
{
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

// 🏁 Build y Middleware
var app = builder.Build();

app.UseMiddleware<ErrorHandlingMiddleware>();

// 🧪 Seeders (solo una vez al arrancar)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await RoleSeeder.SeedRolesAsync(services);
    await UserSeeder.SeedAdminAsync(services);
    await ProductSeeder.SeedTiposProductoAsync(services);
    // await UserSeeder.SeedProveedoresAsync(services); // Descomentá si lo necesitás
}

// 📚 Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Alquimia API V1");
    c.RoutePrefix = string.Empty;
});

app.UseStaticFiles();
app.UseRouting();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.MapControllers();

app.Run();
