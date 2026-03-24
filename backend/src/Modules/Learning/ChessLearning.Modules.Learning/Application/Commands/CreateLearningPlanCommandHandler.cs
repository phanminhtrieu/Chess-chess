using ChessLearning.Modules.Learning.Domain;
using ChessLearning.Modules.Learning.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;

namespace ChessLearning.Modules.Learning.Application.Commands;

public class CreateLearningPlanCommandHandler : ICommandHandler<CreateLearningPlanCommand, Result<Guid>>
{
    private readonly LearningDbContext _dbContext;

    public CreateLearningPlanCommandHandler(LearningDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> ExecuteAsync(CreateLearningPlanCommand command, CancellationToken ct = default)
    {
        var plan = new LearningPlan
        {
            UserId = command.UserId,
            Title = command.Title,
            Description = command.Description,
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Status = LearningPlanStatus.Active
        };

        _dbContext.LearningPlans.Add(plan);
        await _dbContext.SaveChangesAsync(ct);

        return Result<Guid>.Success(plan.Id);
    }
}
