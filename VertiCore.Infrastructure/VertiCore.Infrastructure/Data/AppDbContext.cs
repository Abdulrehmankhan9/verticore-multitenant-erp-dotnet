using Microsoft.EntityFrameworkCore;
using VertiCore.Domain.Entities;
using VertiCore.Application.Interfaces.Services;
using VertiCore.Application.Interfaces.Repositories;

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
        public DbSet<UserInvitation> UserInvitations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Global Query Filters — har query pe evaluate hoga
            modelBuilder.Entity<Client>()
                .HasQueryFilter(c => _currentTenantService!.TenantId == null
                    || c.TenantId == _currentTenantService.TenantId);

            modelBuilder.Entity<Invoice>()
                .HasQueryFilter(i => _currentTenantService!.TenantId == null
                    || i.TenantId == _currentTenantService.TenantId);

            modelBuilder.Entity<InvoiceItem>()
                .HasQueryFilter(ii => _currentTenantService!.TenantId == null
                    || ii.Invoice.TenantId == _currentTenantService.TenantId);

            modelBuilder.Entity<AuditLog>()
                .HasQueryFilter(a => _currentTenantService!.TenantId == null
                    || a.TenantId == _currentTenantService.TenantId);

            modelBuilder.Entity<User>()
                .HasQueryFilter(u => _currentTenantService!.TenantId == null
                    || u.TenantId == _currentTenantService.TenantId);

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
            modelBuilder.Entity<Invoice>()
                .Property(i => i.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.UnitPrice)
                .HasPrecision(18, 2);

            modelBuilder.Entity<InvoiceItem>()
                .Property(i => i.Total)
                .HasPrecision(18, 2);

            base.OnModelCreating(modelBuilder);
        }
    }
}
