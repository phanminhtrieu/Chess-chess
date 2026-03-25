using ChessLearning.Modules.Assignment.Domain;

namespace ChessLearning.Modules.Assignment.Application.Assignments;

public record CreateAssignmentRequest(string CreatedById, string Title, DateTime DueDate);
public record AssignmentDto(Guid Id, string Title, DateTime DueDate, AssignmentStatus Status, IEnumerable<AssignmentTaskDto> Tasks);
public record AssignmentTaskDto(Guid Id, string Title, AssignmentTaskStatus Status, bool IsCompleted);
public record AssignmentDashboardDto(IDictionary<AssignmentStatus, int> Counts, IEnumerable<AssignmentDto> RecentAssignments);
public record TaskInspectionDto(Guid Id, Guid TaskId, AssignmentTaskStatus Status, DateTime CreatedAt, DateTime? CompletedAt, int Order);
public record ContentCorrelationDto(Guid TaskId, Guid ContentId, string ContentType, string Title);
public record AddTaskToAssignmentDto(string Title, string Instructions = "");
