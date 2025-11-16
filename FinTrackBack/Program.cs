// --------------------------
// 📌 USINGS GENERALES
// --------------------------
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

using FinTrackBack.Authentication.Infrastructure.Persistence.DbContext;
using FinTrackBack.Authentication.Application.Interfaces;
using FinTrackBack.Authentication.Infrastructure.Security;

using FinTrackBack.Payments.Domain.Interfaces;
using FinTrackBack.Payments.Infrastructure.Persistence.Repositories;


// --------------------------
// 📌 BUILDER
// --------------------------
var builder = WebApplication.CreateBuilder(args);


// --------------------------
// 📌 SERVICIOS BASE
// --------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


// --------------------------
// 📌 REPOS / DI
// --------------------------
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();


// --------------------------
// 📌 DATABASE: MYSQL
// --------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<FinTrackBackDbContext>(options =>
{
    options.UseMySQL(connectionString);
});


// --------------------------
// 📌 JWT AUTHENTICATION
// --------------------------
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!)
            )
        };
    });

builder.Services.AddAuthorization();


// --------------------------
// 📌 SWAGGER + JWT (Candadito)
// --------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token: Bearer {TU_TOKEN}"
    });

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


// --------------------------
// 📌 APP
// --------------------------
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<FinTrackBackDbContext>();

    // ❗ Si la BD NO existe → la crea
    context.Database.EnsureCreated();
}

// --------------------------
// 📌 PIPELINE
// --------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ORDEN CORRECTO de auth:
app.UseAuthentication();  // Lee el token
app.UseAuthorization();   // Valida el token

app.MapControllers();

app.Run();
