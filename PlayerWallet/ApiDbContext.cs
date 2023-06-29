using PlayerWallet.Models;
using Microsoft.EntityFrameworkCore;

namespace PlayerWallet
{
    public class ApiDbContext : DbContext
    {
        protected override void OnConfiguring
       (DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "PlayerWalletDb");
        }
        public DbSet<Player> Players { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PlayerTransaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PlayerTransaction>().Property(e => e.Id).ValueGeneratedNever();
        }
    }
}