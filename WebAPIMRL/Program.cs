using ClassApplicationMRL.Interfaces;
using ClassApplicationMRL.Services;
using ClassDataMRL.Interfaces;
using ClassDataMRL.Repositories;
using ClassPortafolioMRL.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPIMRL.Middlewares;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();

builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped<IOrdenRepository, OrdenRepository>();

builder.Services.AddScoped<IOrdenDetalleRepository, OrdenDetalleRepository>();


builder.Services.AddScoped<IOrdenService, OrdenService>();



// Repos
builder.Services.AddScoped<IAuthRepository, AuthRepository>();

// JWT
var jwtSection = builder.Configuration.GetSection("Jwt");
var key = Encoding.UTF8.GetBytes(jwtSection["Key"]!.Trim());

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)


    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // true en prod con HTTPS real
        options.SaveToken = true;


        options.TokenValidationParameters = new TokenValidationParameters
        {
            // 🔐 Firma
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),




            // 🏷 Emisor y audiencia
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],

            // ⏱ Expiración
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Administrador"));

    options.AddPolicy("SalesOrAdmin", policy =>
        policy.RequireRole("Administrador", "Vendedor"));

    options.AddPolicy("AdminByIdRol", policy =>
     policy.RequireClaim("IdRol", "1"));
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    // 📘 Información básica de la API
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebAPIMRL API",
        Version = "v1",
        Description = "API REST del sistema MasterPortafolio"
    });

    // 🔐 Configuración JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pega SOLO el token (sin Bearer)"
    });

    // 🔐 Aplicar seguridad global
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});




var app = builder.Build();







// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 👉 Manejo de excepciones
app.UseMiddleware<ExceptionMiddleware>();



app.UseAuthentication();


// 👉 Autorización JWT
app.UseAuthorization();

// 👉 Mapear Controllers
app.MapControllers();

app.Run();




