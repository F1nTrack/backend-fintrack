using FinTrackBack.Authentication.Domain.Entities;
using FinTrackBack.Documents.Domain.Entities;
using FinTrackBack.Documents.Infrastructure.Persistence.DbContext;
using FinTrackBack.Notifications.Domain.Entities;
using FinTrackBack.Notifications.Infrastructure.Persistence.DbContext;
using FinTrackBack.Support.Domain.Entities;
using FinTrackBack.Support.Infrastructure.Persistence.DbContext;
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
    public DbSet<Notification> Notifications { get; set; }  // ✅ NUEVO
    public DbSet<SupportTicket> SupportTickets { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(
            new Documents.Infrastructure.Persistence.DbContext.DocumentConfiguration()
        );
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());   // ✅ NUEVO
        modelBuilder.ApplyConfiguration(new SupportTicketConfiguration());  // ✅ NUEVO
        // Aquí puedes agregar configuraciones de tablas en el futuro
    }
}