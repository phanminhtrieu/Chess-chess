using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;
using ChessLearning.Modules.Identity.Domain;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record SetRoleCommand(Guid UserId, string Role) : ICommand;

public class SetRoleCommandHandler : ICommandHandler<SetRoleCommand>
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;

    public SetRoleCommandHandler(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task ExecuteAsync(SetRoleCommand command, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(command.UserId.ToString());
        if (user == null)
            throw new Exception("User not found: " + command.UserId);

        var roles = await _userManager.GetRolesAsync(user);
        
        // If user already has the requested role, just succeed (idempotency)
        if (roles.Contains(command.Role))
        {
            return;
        }

        if (!await _roleManager.RoleExistsAsync(command.Role))
            throw new Exception($"Role {command.Role} does not exist.");

        // Restrict to Student/Teacher for this command
        if (command.Role != "Student" && command.Role != "Teacher")
            throw new Exception("Only Student or Teacher roles can be selected.");

        var result = await _userManager.AddToRoleAsync(user, command.Role);
        if (!result.Succeeded)
            throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
    }
}
