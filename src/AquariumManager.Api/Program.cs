using System.Text;
using System.Text.Json.Serialization;
using AquariumManager.Application.Common;
using AquariumManager.Application.Services;
using AquariumManager.Domain.Interfaces;
using AquariumManager.Infrastructure.Persistence;
using AquariumManager.Infrastructure.Repositories;
using AquariumManager.Infrastructure.UnitOfWork;
using AquariumManager.Api.Middleware;
using AquariumManager.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

// Load .env file if present (maps flat keys to structured config)
var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (File.Exists(envPath))
{
    foreach (var line in File.ReadAllLines(envPath))
    {
        var trimmed = line.Trim();
        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
            continue;

        var eq = trimmed.IndexOf('=');
        if (eq < 0)
            continue;

        var envKey = trimmed[..eq].Trim();
        var value = trimmed[(eq + 1)..].Trim();

        if (string.IsNullOrWhiteSpace(envKey) || string.IsNullOrWhiteSpace(value))
            continue;
        // Map flat .env keys to structured config sections
        switch (envKey)
        {
            case "DB_CONNECTION":
            case "Db__Connection":
            case "ConnectionStrings__DefaultConnection":
                builder.Configuration["ConnectionStrings:DefaultConnection"] = value;
                break;
            case "JWT_KEY":
            case "Jwt__Key":
            case "JWT__KEY":
                builder.Configuration["Jwt:Key"] = value;
                break;
            case "JWT_ISSUER":
            case "Jwt__Issuer":
            case "JWT__ISSUER":
                builder.Configuration["Jwt:Issuer"] = value;
                break;
            case "JWT_AUDIENCE":
            case "Jwt__Audience":
            case "JWT__AUDIENCE":
                builder.Configuration["Jwt:Audience"] = value;
                break;
            case "CORS_ORIGIN":
            case "Cors__Origin":
            case "CORS__ORIGIN":
                builder.Configuration["Cors:Origin"] = value;
                break;
        }
    }
}

var myCorsPolicy = "_myCorsPolicy";

// CORS
var corsOrigin = builder.Configuration["Cors:Origin"] ?? "http://localhost:5173";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myCorsPolicy, policy =>
    {
        policy
            .WithOrigins(corsOrigin)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DbContext
builder.Services.AddDbContext<AquariumDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    options.UseNpgsql(connectionString);
});

// Repositorios
builder.Services.AddScoped<ITenantRepository, TenantRepository>();
builder.Services.AddScoped<ISpeciesRepository, SpeciesRepository>();
builder.Services.AddScoped<ISpeciesVariantRepository, SpeciesVariantRepository>();
builder.Services.AddScoped<IInventoryLotRepository, InventoryLotRepository>();
builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
builder.Services.AddScoped<ISaleRepository, SaleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Tank module repositories
builder.Services.AddScoped<ITankRepository, TankRepository>();
builder.Services.AddScoped<IWaterParameterLogRepository, WaterParameterLogRepository>();
builder.Services.AddScoped<IMaintenanceLogRepository, MaintenanceLogRepository>();
builder.Services.AddScoped<IFertilizationLogRepository, FertilizationLogRepository>();
builder.Services.AddScoped<ITankPhotoRepository, TankPhotoRepository>();
builder.Services.AddScoped<IFertilizerPresetRepository, FertilizerPresetRepository>();
builder.Services.AddScoped<ITargetParameterRangeRepository, TargetParameterRangeRepository>();
builder.Services.AddScoped<IAlertConfigRepository, AlertConfigRepository>();

// Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Current user / tenant context
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

// Servicios de aplicación
builder.Services.AddScoped<ISpeciesService, SpeciesService>();
builder.Services.AddScoped<ISpeciesVariantService, SpeciesVariantService>();
builder.Services.AddScoped<IInventoryLotService, InventoryLotService>();
builder.Services.AddScoped<ISupplierService, SupplierService>();
builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<ISaleService, SaleService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAlertService, AlertService>();

// Tank module services
builder.Services.AddScoped<ITankService, TankService>();
builder.Services.AddScoped<IFertilizerPresetService, FertilizerPresetService>();

// JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
    };
});

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("OwnerOnly", policy => policy.RequireRole("Owner"));
// Controllers
builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global error handler
app.UseMiddleware<GlobalExceptionHandler>();

// CORS
app.UseCors(myCorsPolicy);
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
