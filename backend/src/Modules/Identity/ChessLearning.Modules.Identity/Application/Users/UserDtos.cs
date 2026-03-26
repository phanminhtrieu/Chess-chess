using ChessLearning.Modules.Identity.Domain;

namespace ChessLearning.Modules.Identity.Application.Users;

public record UserProfileDto(string Id, string DisplayName, string Email, string Role);

public record UserListItemDto(string Id, string DisplayName, string Email, string Role, bool IsActive, DateTime CreatedAt, DateTime UpdatedAt);

public record UserAuditDto(string Id, string DisplayName, string Email, DateTime CreatedAt, DateTime UpdatedAt, IEnumerable<RefreshTokenDto> RefreshTokens);

public record RefreshTokenDto(string Token, DateTime ExpiresAt, DateTime? RevokedAt, bool IsActive);
