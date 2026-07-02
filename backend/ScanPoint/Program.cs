using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using AutoMapper;
using ScanPoint.Repositories.Interfaces;
using ScanPoint.Repositories.Repositories;
using ScanPoint.Models.Data;
using ScanPoint.Models.Mappings;
using ScanPoint.Repositories;

using System.Text;



var builder = WebApplication.CreateBuilder(args);

// -------------------- SERVICES --------------------


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });


// DbContext
builder.Services.AddDbContext<ScanPointDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"), sqlServerOptionsAction: sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(); // Kjo e ndihmon lidhjen të mos dështojë menjëherë
    }));

// -------------------- JWT Authentication --------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            // ClockSkew = Zero: token skadon SAKTËSISHT në kohën e vendosur (30min),
            // jo 5min ekstra si default i ASP.NET Core.
            ClockSkew = TimeSpan.Zero,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuerSigningKey = true
        };

        // Kthen 401 me mesazh të qartë kur token mungon/është i pavlefshëm
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new { message = "Nuk jeni të autorizuar. Token mungon ose është i pavlefshëm." });
                return context.Response.WriteAsync(result);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.ContentType = "application/json";
                var result = System.Text.Json.JsonSerializer.Serialize(
                    new { message = "Nuk keni leje për këtë veprim." });
                return context.Response.WriteAsync(result);
            }
        };
    });

builder.Services.AddAuthorization();

// -------------------- Dependency Injection --------------------
builder.Services.AddScoped<IAuthService, AuthRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IShopRepository, ShopRepository>();
builder.Services.AddScoped<IManagerRepository, ManagerRepository>();
builder.Services.AddScoped<ICashierRepository, CashierRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IUserManagementService, UserManagementService>();
builder.Services.AddScoped<IManagerService, ManagerService>();
//                                          ketu ki me shtu
// ----------------------------====================================================-Dependency Injection — kur Controller kërkon IShkollaRepository, jep ShkollaRepository
// "Scoped" = krijohet një instancë për çdo request HTTP






// AutoMapper
builder.Services.AddAutoMapper(typeof(ManagerProfile).Assembly);

// -------------------- Swagger --------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "ScanPoint API",
        Version = "v1",
        Description = "API me JWT dhe Scalar support"
    });

    c.OperationFilter<FileUploadOperationFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Vendos JWT token me prefix 'Bearer '"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// -------------------- CORS --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy
                .WithOrigins("http://localhost:5173")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});


builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5055);
});
// -------------------- BUILD APP --------------------
var app = builder.Build();

// -------------------- MIDDLEWARE --------------------
// RËNDËSI: Rendi i middleware-it është kritik!

//app.UseHttpsRedirection();

app.UseStaticFiles();

// CORS duhet para Authentication
app.UseCors("AllowFrontend");

// Authentication para Authorization — GJITHMONË në këtë rend
app.UseAuthentication();
app.UseAuthorization();

// Swagger
app.UseSwagger(c =>
{
    c.RouteTemplate = "openapi/{documentName}.json";
});

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/openapi/v1.json", "ScanPoint API V1");
    c.RoutePrefix = "swagger";
    c.DisplayRequestDuration();
});

app.MapScalarApiReference();

app.MapControllers();

app.Run();