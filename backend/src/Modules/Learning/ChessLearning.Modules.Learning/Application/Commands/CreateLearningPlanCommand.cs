using ChessLearning.Shared.Application.Contracts.Cqrs;

namespace ChessLearning.Modules.Learning.Application.Commands;

public record CreateLearningPlanCommand(
    Guid UserId, 
    string Title, 
    string Description, 
    DateTime StartDate, 
    DateTime EndDate
) : ICommand<Result<Guid>>;
