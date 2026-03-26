using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record UpdateUserCommand(Guid Id, string? DisplayName, string? Role) : ICommand;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand>
{
    private readonly UserManager<User> _userManager;

    public UpdateUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task ExecuteAsync(UpdateUserCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByIdAsync(command.Id.ToString());
        if (user == null) throw new Exception("User not found");

        if (!string.IsNullOrEmpty(command.DisplayName) && user.DisplayName != command.DisplayName)
        {
            user.DisplayName = command.DisplayName;
        }

        user.SetUpdatedAt();
        await _userManager.UpdateAsync(user);

        if (!string.IsNullOrEmpty(command.Role))
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (!currentRoles.Contains(command.Role))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, command.Role);
            }
        }
    }
}
