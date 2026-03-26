using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Modules.Identity.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Identity.Application.Users.Queries;

public record GetUserAuditQuery(Guid UserId) : IQuery<UserAuditDto>;

public class GetUserAuditQueryHandler : IQueryHandler<GetUserAuditQuery, UserAuditDto>
{
    private readonly UserManager<User> _userManager;
    private readonly IdentityDbContext _db;

    public GetUserAuditQueryHandler(UserManager<User> userManager, IdentityDbContext db)
    {
        _userManager = userManager;
        _db = db;
    }

    public async Task<UserAuditDto> HandleAsync(GetUserAuditQuery query, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(query.UserId.ToString());
        if (user == null) throw new Exception("User not found");

        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == query.UserId)
            .OrderByDescending(rt => rt.CreatedAt)
            .Select(rt => new RefreshTokenDto(rt.Token, rt.ExpiresAt, rt.RevokedAt, rt.IsActive))
            .ToListAsync(ct);

        return new UserAuditDto(user.Id.ToString(), user.DisplayName ?? string.Empty, user.Email!, user.CreatedAt, user.UpdatedAt, tokens);
    }
}
