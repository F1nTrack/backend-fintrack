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


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<ISupportTicketRepository, SupportTicketRepository>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var environment = builder.Environment.EnvironmentName;

builder.Services.AddDbContext<FinTrackBackDbContext>(options =>
{
    if (environment == "Development")
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseMySql(connectionString,
            new MySqlServerVersion(new Version(8, 0, 21)), 
            mySqlOptions => mySqlOptions.SchemaBehavior(MySqlSchemaBehavior.Ignore));
    }
});

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly)
);

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

var app = builder.Build();

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

app.MapControllers();

app.Run();
