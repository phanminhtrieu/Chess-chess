using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record DeactivateUserCommand(Guid UserId) : ICommand;

public class DeactivateUserCommandHandler : ICommandHandler<DeactivateUserCommand>
{
    private readonly UserManager<User> _userManager;

    public DeactivateUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task ExecuteAsync(DeactivateUserCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null) throw new Exception("User not found");

        user.IsActive = false;
        user.SetUpdatedAt();
        await _userManager.UpdateAsync(user);
    }
}
