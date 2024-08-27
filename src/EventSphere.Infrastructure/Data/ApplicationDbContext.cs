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
    
    /// <summary>
    /// The events comments table in the database
    /// </summary>
    public DbSet<Comment> Comments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureEvent(modelBuilder);
        ConfigureLike(modelBuilder);
        ConfigureComment(modelBuilder);
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

    private static void ConfigureComment(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Comment>()
            .Ignore(e => e.User);
        
        modelBuilder.Entity<Comment>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Comment>()
            .HasOne<Event>()
            .WithMany()
            .HasForeignKey(c => c.EventId);

        modelBuilder.Entity<Comment>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(c => c.UserId);
    }
}