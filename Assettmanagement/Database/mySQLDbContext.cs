using Microsoft.EntityFrameworkCore;
using Assettmanagement.Models;

namespace Assettmanagement.Database
{
    public class mySQLDbContext : DbContext
    {
        public DbSet<Asset> Assets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<AssetHistory> AssetHistories { get; set; }

        public mySQLDbContext(DbContextOptions<mySQLDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(ah => ah.Asset)
                .WithMany()
                .HasForeignKey(ah => ah.AssetId);

            modelBuilder.Entity<AssetHistory>()
                .HasOne(ah => ah.User)
                .WithMany()
                .HasForeignKey(ah => ah.UserId);
        }
    }
}
