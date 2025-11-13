using FinTrackBack.Authentication.Domain.Entities;
using FinTrackBack.Payments.Domain.Entities; // 👈 referencia cruzada al módulo de pagos
using Microsoft.EntityFrameworkCore;

namespace FinTrackBack.Authentication.Infrastructure.Persistence.DbContext
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
}