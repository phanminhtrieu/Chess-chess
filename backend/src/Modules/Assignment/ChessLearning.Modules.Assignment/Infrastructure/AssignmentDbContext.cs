using ChessLearning.Modules.Assignment.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Assignment.Infrastructure;

public class AssignmentDbContext : DbContext
{
    public AssignmentDbContext(DbContextOptions<AssignmentDbContext> options) : base(options)
    {
    }

    public DbSet<Domain.Assignment> Assignments => Set<Domain.Assignment>();
    public DbSet<AssignmentTask> AssignmentTasks => Set<AssignmentTask>();
    public DbSet<LearningTask> Tasks => Set<LearningTask>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("assignment");

        modelBuilder.Entity<Domain.Assignment>()
            .HasMany(a => a.AssignmentTasks)
            .WithOne(at => at.Assignment)
            .HasForeignKey(at => at.AssignmentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<AssignmentTask>()
            .HasOne(at => at.Task)
            .WithMany()
            .HasForeignKey(at => at.TaskId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
