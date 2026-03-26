using ChessLearning.Modules.Identity.Infrastructure;
using ChessLearning.Shared.Application.Contracts;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record LogoutCommand(string RefreshToken) : ICommand;

public class LogoutCommandHandler : ICommandHandler<LogoutCommand>
{
    private readonly IdentityDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(IdentityDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(LogoutCommand command, CancellationToken ct = default)
    {
        if (!_currentUser.IsAuthenticated)
            return;

        var token = await _db.RefreshTokens
            .Where(rt => rt.UserId == _currentUser.UserId && rt.Token == command.RefreshToken && rt.RevokedAt == null)
            .FirstOrDefaultAsync(ct);

        if (token != null)
        {
            token.RevokedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }
}
