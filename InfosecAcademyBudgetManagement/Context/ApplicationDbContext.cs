using Cost;
using InfosecAcademyBudgetManagement.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace InfosecAcademyBudgetManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<CostItem> CostItems { get; set; }
        public DbSet<IncomeItem> IncomeItems { get; set; }
        public DbSet<Counterparty> Counterparties { get; set; }
        public DbSet<PurchaseRequest> PurchaseRequests { get; set; }
        public DbSet<PaymentScheduleItem> PaymentScheduleItems { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BudgetPlan> BudgetPlans { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<EmailAccountSetting> EmailAccountSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
                entity.Property(u => u.Department).HasMaxLength(100);
            });

            modelBuilder.Entity<EmailAccountSetting>(entity =>
            {
                entity.Property(e => e.SmtpHost).HasMaxLength(200).IsRequired();
                entity.Property(e => e.SenderEmail).HasMaxLength(256).IsRequired();
                entity.Property(e => e.SenderName).HasMaxLength(100);
                entity.Property(e => e.Username).HasMaxLength(256).IsRequired();
                entity.Property(e => e.EncryptedPassword).IsRequired();
            });

            modelBuilder.Entity<Counterparty>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(150).IsRequired();
                entity.Property(c => c.Type).HasMaxLength(30).IsRequired();
                entity.Property(c => c.TaxNumber).HasMaxLength(30);
                entity.Property(c => c.Phone).HasMaxLength(30);
                entity.Property(c => c.Email).HasMaxLength(120);
                entity.Property(c => c.Address).HasMaxLength(250);
                entity.Property(c => c.Note).HasMaxLength(500);
            });

            modelBuilder.Entity<BudgetPlan>(entity =>
            {
                entity.Property(b => b.VersionNo).HasDefaultValue(1);
                entity.HasIndex(b => new { b.Year, b.VersionNo }).IsUnique();
            });

            modelBuilder.Entity<PurchaseRequest>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(200).IsRequired();
                entity.Property(p => p.Status).HasMaxLength(30).IsRequired();
                entity.Property(p => p.RejectionReason).HasMaxLength(500);
            });

            modelBuilder.Entity<PaymentScheduleItem>(entity =>
            {
                entity.Property(p => p.Title).HasMaxLength(150).IsRequired();
                entity.Property(p => p.Type).HasMaxLength(20).IsRequired();
                entity.HasOne(p => p.Counterparty)
                    .WithMany()
                    .HasForeignKey(p => p.CounterpartyId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<AuditLog>(entity =>
            {
                entity.Property(a => a.Method).HasMaxLength(10).IsRequired();
                entity.Property(a => a.Path).HasMaxLength(400).IsRequired();
                entity.Property(a => a.UserId).HasMaxLength(100);
                entity.Property(a => a.UserName).HasMaxLength(200);
                entity.Property(a => a.IpAddress).HasMaxLength(50);
                entity.Property(a => a.Detail).HasMaxLength(500);
            });

            modelBuilder.Entity<CostItem>()
                .Property(c => c.CategoryId)
                .HasDefaultValue(5);

            modelBuilder.Entity<BudgetLine>()
                .HasIndex(b => new { b.BudgetPlanId, b.CategoryId, b.Month })
                .IsUnique();

            var seedTime = new DateTime(2026, 02, 06, 0, 0, 0, DateTimeKind.Utc);
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Fixed", CreatedAt = seedTime, UpdatedAt = seedTime, IsDeleted = false },
                new Category { Id = 2, Name = "Utilities", CreatedAt = seedTime, UpdatedAt = seedTime, IsDeleted = false },
                new Category { Id = 3, Name = "Office", CreatedAt = seedTime, UpdatedAt = seedTime, IsDeleted = false },
                new Category { Id = 4, Name = "Travel", CreatedAt = seedTime, UpdatedAt = seedTime, IsDeleted = false },
                new Category { Id = 5, Name = "Other", CreatedAt = seedTime, UpdatedAt = seedTime, IsDeleted = false }
            );
        }
    }
}
