using ChessLearning.Modules.Progress.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Progress.Infrastructure;

public class ProgressDbContext : DbContext
{
    public ProgressDbContext(DbContextOptions<ProgressDbContext> options) : base(options)
    {
    }

    public DbSet<ProgressEvent> ProgressEvents => Set<ProgressEvent>();
    public DbSet<ProgressSummary> ProgressSummaries => Set<ProgressSummary>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("progress");

        modelBuilder.Entity<ProgressEvent>()
            .HasIndex(e => new { e.UserId, e.Timestamp });
    }
}
