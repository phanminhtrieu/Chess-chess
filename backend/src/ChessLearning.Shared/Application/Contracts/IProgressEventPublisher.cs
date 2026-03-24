namespace ChessLearning.Shared.Application.Contracts;

public interface IProgressEventPublisher
{
    Task PublishAsync(ProgressEventData eventData, CancellationToken ct = default);
}

public record ProgressEventData(
    Guid UserId,
    string Type,
    double Value,
    object? Metadata = null,
    Guid? TenantId = null
);
