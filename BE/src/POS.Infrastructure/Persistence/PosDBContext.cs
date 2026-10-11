using Microsoft.EntityFrameworkCore;
using POS.Domain.Common;
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
        public DbSet<CodeSequence> CodeSequences => Set<CodeSequence>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PosDBContext).Assembly);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes()
             .Where(t => typeof(EntityBase).IsAssignableFrom(t.ClrType)))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property(nameof(EntityBase.Id))
                    .ValueGeneratedNever();
            }
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
        }

        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void ApplyAuditInfo()
        {
            // TODO: replace "System" with the authenticated user's id/username
            // once auth (e.g. an ICurrentUserService injected here) is wired up.
            const string currentUser = "System";
            var utcNow = DateTime.UtcNow;

            var entries = ChangeTracker.Entries<EntityBase>()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = currentUser;
                }
                else
                {
                    // Don't let an update accidentally overwrite creation audit fields
                    entry.Property(nameof(EntityBase.CreatedAt)).IsModified = false;
                    entry.Property(nameof(EntityBase.CreatedBy)).IsModified = false;
                }

                entry.Entity.UpdatedAt = utcNow;
                entry.Entity.UpdatedBy = currentUser;
            }
        }
    }
}
