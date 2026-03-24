using ChessLearning.Modules.Game.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Game.Infrastructure;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
    {
    }

    public DbSet<ImportedGame> ImportedGames => Set<ImportedGame>();
    public DbSet<GameImportJob> GameImportJobs => Set<GameImportJob>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("game");

        modelBuilder.Entity<ImportedGame>()
            .HasIndex(g => new { g.UserId, g.Source, g.ExternalId })
            .IsUnique();
    }
}
