using Microsoft.EntityFrameworkCore;
using Assettmanagement.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<AssetHistory> AssetHistories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Seed the default system user "qwerty"
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = 1, // Use a specific ID for the system user
                FirstName = "System",
                LastName = "System",
                Email = "system@home.com",
                PasswordHash = "65e84be33532fb784c48129675f9eff3a682b27168c0ea744b2cf58ee02337c5", // You should hash a real password
                IsAdministrator = true
                // Add other required fields as necessary
            }
        );
    }
}
