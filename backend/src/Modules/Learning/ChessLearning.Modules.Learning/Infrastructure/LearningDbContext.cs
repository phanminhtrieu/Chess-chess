using ChessLearning.Modules.Learning.Domain;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Learning.Infrastructure;

public class LearningDbContext : DbContext
{
    public LearningDbContext(DbContextOptions<LearningDbContext> options) : base(options)
    {
    }

    public DbSet<LearningPlan> LearningPlans => Set<LearningPlan>();
    public DbSet<DailyGoal> DailyGoals => Set<DailyGoal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Use default schema for modular monolith (could configure separate schemas per module if desired)
        modelBuilder.HasDefaultSchema("learning");

        modelBuilder.Entity<LearningPlan>()
            .HasMany(lp => lp.DailyGoals)
            .WithOne(dg => dg.LearningPlan)
            .HasForeignKey(dg => dg.LearningPlanId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
