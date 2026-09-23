using Microsoft.EntityFrameworkCore;
using POS.Domain.Entities;

namespace POS.Infrastructure.Persistence
{
    public class PosDBContext : DbContext
    {
        public PosDBContext(DbContextOptions<PosDBContext> options) : base(options)
        {
        }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PosDBContext).Assembly);
        }

    }
}