using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReenbitChat.Data.Entities;

namespace ReenbitChat.Data.Contexts;

/// <summary>
/// The main Entity Framework Core database context for the application.
/// Integrates ASP.NET Core Identity for user management and defines DbSets for chat entities.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    /// <summary> Collection of all chat messages. </summary>
    public DbSet<Message> Messages { get; set; }
    
    /// <summary> Collection of all created chat rooms. </summary>
    public DbSet<ChatRoom> ChatRooms { get; set; }

    /// <summary>
    /// Configures the database schema, entity relationships, and delete behaviors.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure ChatRoom -> Creator relationship
        // If a user is deleted, their created rooms remain, but CreatorId becomes null
        builder.Entity<ChatRoom>()
            .HasOne(c => c.Creator)
            .WithMany()
            .HasForeignKey(c => c.CreatorId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Message -> User relationship
        // If a user is deleted, their messages remain to preserve chat history (UserId becomes null)
        builder.Entity<Message>()
            .HasOne(m => m.User)
            .WithMany() 
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        // Configure Message -> ChatRoom relationship
        // If a chat room is deleted, all messages inside it are cascade deleted
        builder.Entity<Message>()
            .HasOne(m => m.ChatRoom)
            .WithMany() 
            .HasForeignKey(m => m.ChatRoomId)
            .OnDelete(DeleteBehavior.Cascade);
    }

    /// <summary>
    /// Overrides the default save behavior to automatically handle CreatedAt and UpdatedAt 
    /// audit properties for all entities inheriting from BaseEntity.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseEntity && 
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            var entity = (BaseEntity)entityEntry.Entity;

            if (entityEntry.State == EntityState.Added)
            {
                entity.CreatedAt = DateTimeOffset.UtcNow;
            }
            else if (entityEntry.State == EntityState.Modified)
            {
                entity.UpdatedAt = DateTimeOffset.UtcNow;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}