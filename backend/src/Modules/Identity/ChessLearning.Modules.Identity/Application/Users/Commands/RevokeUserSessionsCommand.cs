using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Modules.Identity.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record RevokeUserSessionsCommand(Guid UserId) : ICommand;

public class RevokeUserSessionsCommandHandler : ICommandHandler<RevokeUserSessionsCommand>
{
    private readonly IdentityDbContext _db;
    private readonly UserManager<User> _userManager;

    public RevokeUserSessionsCommandHandler(IdentityDbContext db, UserManager<User> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public async Task ExecuteAsync(RevokeUserSessionsCommand command, CancellationToken ct = default)
    {
        var tokens = await _db.RefreshTokens
            .Where(rt => rt.UserId == command.UserId && rt.RevokedAt == null)
            .ToListAsync(ct);

        foreach (var token in tokens)
        {
            token.RevokedAt = DateTime.UtcNow;
        }

        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user != null)
        {
            user.IsActive = false;
            await _userManager.UpdateAsync(user);
        }

        await _db.SaveChangesAsync(ct);
    }
}
