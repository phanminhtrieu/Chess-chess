using ChessLearning.Modules.Content.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Content.Infrastructure;

public class ContentDbContext : DbContext
{
    public ContentDbContext(DbContextOptions<ContentDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Content> Contents => Set<Domain.Content>();
    public DbSet<Puzzle> Puzzles => Set<Puzzle>();
    public DbSet<PuzzleAttempt> PuzzleAttempts => Set<PuzzleAttempt>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("content");

        modelBuilder.Entity<Domain.Content>()
            .HasOne(c => c.Puzzle)
            .WithOne(p => p.Content)
            .HasForeignKey<Puzzle>(p => p.ContentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Puzzle>()
            .HasIndex(p => new { p.ExternalId, p.ExternalSource })
            .IsUnique();
    }
}
