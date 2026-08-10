
using Domain.Entities;
using Domain.Entities.Common;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public new DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            ApplyAudit();

            var result = await base.SaveChangesAsync(cancellationToken);

            return result;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        private void ApplyAudit()
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>())
            {
                const string defaultUser = "SYSTEM";

                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    if (string.IsNullOrWhiteSpace(entry.Entity.CreatedBy))
                        entry.Entity.CreatedBy = defaultUser;
                }

                if (entry.State is EntityState.Added or EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = now;
                    if (string.IsNullOrWhiteSpace(entry.Entity.UpdatedBy))
                        entry.Entity.UpdatedBy = defaultUser;
                }
            }
        }
    }
}