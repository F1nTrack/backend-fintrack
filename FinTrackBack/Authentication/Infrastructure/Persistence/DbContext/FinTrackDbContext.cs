using FinTrackBack.Authentication.Domain.Entities;
using FinTrackBack.Payments.Domain.Entities; // 👈 referencia cruzada al módulo de pagos


namespace FinTrackBack.Authentication.Infrastructure.Persistence.DbContext
using FinTrackBack.Documents.Domain.Entities;
using FinTrackBack.Documents.Infrastructure.Persistence.DbContext;
using FinTrackBack.Notifications.Domain.Entities;
using FinTrackBack.Notifications.Infrastructure.Persistence.DbContext;
using FinTrackBack.Support.Domain.Entities;
using FinTrackBack.Support.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
namespace FinTrackBack.Authentication.Infrastructure.Persistence.DbContext;

public class FinTrackBackDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public class FinTrackBackDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public FinTrackBackDbContext(DbContextOptions<FinTrackBackDbContext> options) : base(options)
        {
        }

        // 👤 Usuarios
        public DbSet<User> Users { get; set; }

        // 💳 Pagos (viene del módulo Payments)
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapeo explícito (opcional)
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Payment>().ToTable("Payments");
        }
    }
    
    public DbSet<User> Users { get; set; }
    public DbSet<Documents.Domain.Entities.Document> Documents { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<SupportTicket> SupportTickets { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(
            new Documents.Infrastructure.Persistence.DbContext.DocumentConfiguration()
        );
        modelBuilder.ApplyConfiguration(new NotificationConfiguration());   
        modelBuilder.ApplyConfiguration(new SupportTicketConfiguration());  
    }
}
