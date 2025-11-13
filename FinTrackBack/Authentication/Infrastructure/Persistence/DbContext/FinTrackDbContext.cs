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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Aquí puedes agregar configuraciones de tablas en el futuro
    }
}