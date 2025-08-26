using learning.Entities;
using Microsoft.EntityFrameworkCore;

namespace learning.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        // Generic: Apply default timestamp for CreatedDate if exists
        foreach (var entityType in modelBuilder.Model.GetEntityTypes()) {
            var property = entityType.ClrType.GetProperty("CreatedDate");
            if (property != null && property.PropertyType == typeof(DateTime))
            {
                modelBuilder.Entity(entityType.ClrType)
                    .Property("CreatedDate")
                    .HasColumnType("timestamp")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            }
        }
    }
}