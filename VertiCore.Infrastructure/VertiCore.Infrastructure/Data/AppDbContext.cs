using Microsoft.EntityFrameworkCore;
using VertiCore.Domain.Entities;
using VertiCore.Application.Interfaces;

namespace VertiCore.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentTenantService? _currentTenantService;

        public AppDbContext(DbContextOptions<AppDbContext> options,
            ICurrentTenantService? currentTenantService = null)
            : base(options)
        {
            _currentTenantService = currentTenantService;
        }

        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceItem> InvoiceItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var tenantId = _currentTenantService?.TenantId;

            // Global Query Filters — Multi-tenancy
            modelBuilder.Entity<Client>()
                .HasQueryFilter(c => tenantId == null || c.TenantId == tenantId);

            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(i => tenantId == null || i.TenantId == tenantId);

            modelBuilder.Entity<AuditLog>()
                .HasQueryFilter(a => tenantId == null || a.TenantId == tenantId);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => tenantId == null || u.TenantId == tenantId);

            // Cascade fix
            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Tenant)
                .WithMany()
                .HasForeignKey(i => i.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Invoice>()
                .HasOne(i => i.Client)
                .WithMany()
                .HasForeignKey(i => i.ClientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.Tenant)
                .WithMany()
                .HasForeignKey(a => a.TenantId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<AuditLog>()
                .HasOne(a => a.PerformedBy)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
