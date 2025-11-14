
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using FinTrackBack.Authentication.Infrastructure.Persistence.DbContext;
using FinTrackBack.Authentication.Application.Interfaces;
using FinTrackBack.Authentication.Infrastructure.Security;




using FinTrackBack.Notifications.Domain.Interfaces;
using FinTrackBack.Notifications.Infrastructure.Persistence.Repositories;

using FinTrackBack.Support.Domain.Interfaces;
using FinTrackBack.Support.Infrastructure.Persistence.Repositories;



using FinTrackBack.Documents.Domain.Interfaces;
using FinTrackBack.Documents.Infrastructure.Persistence.Repositories;



// ---USING INYECTIONS
using FinTrackBack.Payments.Domain.Interfaces;
using FinTrackBack.Payments.Infrastructure.Persistence.Repositories;

// --- USINGS NUEVOS PARA JWT Y SWAGGER ---
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
// --- FIN USINGS NUEVOS ---

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

//SERVICIO DE ADICION
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
// --- MODIFICACIÓN DE SWAGGER (PARA AÑADIR CANDADO DE AUTORIZACIÓN) ---
builder.Services.AddSwaggerGen(options =>
{
    // 1. Definir la seguridad (Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT obtenido en el login: 'Bearer {token}'"
    });

    // 2. Hacer que Swagger use esa definición
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] {}
        }
    });
});
// --- FIN MODIFICACIÓN SWAGGER ---


// --- CONEXIÓN A MYSQL (Existente) ---

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                       ?? builder.Configuration.GetConnectionString("FinTrackDatabase");

builder.Services.AddDbContext<FinTrackBackDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 21)),
        mySqlOptions =>
        {
            mySqlOptions.EnableRetryOnFailure();
        }
    )
);



// --- PEGAMENTO DI (Existente) ---
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
);
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


// --- NUEVO: AÑADIR SERVICIOS DE AUTENTICACIÓN JWT ---
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Le decimos a la API cómo validar el token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

// Habilita el uso de [Authorize] en los controladores
builder.Services.AddAuthorization();
// --- FIN SERVICIOS NUEVOS ---


var app = builder.Build();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinTrackBackDbContext>();
    dbContext.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- NUEVO: AÑADIR MIDDLEWARE DE AUTENTICACIÓN ---
// ¡Importante! Deben ir ANTES de MapControllers.
// Este "lee" el token en cada petición
app.UseAuthentication();
// Este "valida" el token si el endpoint tiene [Authorize]
app.UseAuthorization();
// --- FIN MIDDLEWARE NUEVO ---


app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FinTrackBackDbContext>();
    db.Database.Migrate();
}


app.Run();
