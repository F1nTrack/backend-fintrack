using FinTrackBack.Authentication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

// Asegúrate de que el namespace sea EXACTAMENTE este.
namespace FinTrackBack.Authentication.Infrastructure.Persistence.DbContext;

public class FinTrackBackDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public FinTrackBackDbContext(DbContextOptions<FinTrackBackDbContext> options) : base(options)
    {
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Documents.Domain.Entities.Document> Documents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(
            new Documents.Infrastructure.Persistence.DbContext.DocumentConfiguration()
        );

        // Aquí puedes agregar configuraciones de tablas en el futuro
    }
}