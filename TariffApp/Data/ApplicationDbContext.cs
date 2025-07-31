using Microsoft.EntityFrameworkCore;
using TariffApp.Models;
using TariffApp.Models.Enum;

namespace TariffApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Client> Clients { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Sender)
                .WithMany()
                .HasForeignKey(t => t.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Receiver)
                .WithMany()
                .HasForeignKey(t => t.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Configuration>().HasData(
            new Configuration
            {
                Id = 1,
                DefaultCurrency = Currency.EUR,
                MaxFeeLimit = 120.0,
                PosFixedFee = 0.20,
                PosPercentFee = 0.002,
                CreditScoreDiscount = 400,
                LastUpdated = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc)
            }
);

        }
        public DbSet<TariffApp.Models.Configuration> Configuration { get; set; } = default!;
    }
}
