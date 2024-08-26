using EventSphere.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EventSphere.Infrastructure.Data;

/// <summary>
/// The database context service used for interaction with the database
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    /// <summary>
    /// The Users table in the database
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// The Events table in the database
    /// </summary>
    public DbSet<Event> Events { get; set; }
    
    /// <summary>
    /// The events likes table in the database
    /// </summary>
    public DbSet<Like> Likes { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureEvent(modelBuilder);
        ConfigureLike(modelBuilder);
    }

    private static void ConfigureEvent(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .Ignore(e => e.LikesCount);
    }

    private static void ConfigureLike(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Like>()
            .HasKey(l => new { l.EventId, l.UserId });
        
        modelBuilder.Entity<Like>()
            .HasOne<Event>()
            .WithMany()
            .HasForeignKey(l => l.EventId);

        modelBuilder.Entity<Like>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(l => l.UserId);
    }
}