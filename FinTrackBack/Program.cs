
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

// 1. Controllers y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT: 'Bearer {token}'"
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

// 2. Repositorios
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();

// 3. Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

Console.WriteLine("Environment: " + builder.Environment.EnvironmentName);
Console.WriteLine("Connection String: " + (connectionString ?? "EMPTY!"));

// Validar si está vacío
if (string.IsNullOrWhiteSpace(connectionString))
{
    Console.WriteLine("❌ ERROR: La cadena de conexión está vacía o no se encontró.");
}

// AddDbContext CORRECTO — solo uno
builder.Services.AddDbContext<FinTrackBackDbContext>(options =>
    options.UseMySql(
        connectionString,
        new MySqlServerVersion(new Version(8, 0, 36)),   // compatible Railway
        mysql => mysql.EnableRetryOnFailure()
    )
);


// 4. JWT
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
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Railway PORT
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
app.Urls.Add($"http://0.0.0.0:{port}");

// MIGRACIONES — SOLO 1 VEZ
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FinTrackBackDbContext>();
    dbContext.Database.Migrate();
}

app.UseDeveloperExceptionPage();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";

        var error = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

        if (error != null)
        {
            await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                error = error.Error.Message,
                stack = error.Error.StackTrace
            }));
        }
    });
});


app.UseSwagger();
app.UseSwaggerUI();



app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();