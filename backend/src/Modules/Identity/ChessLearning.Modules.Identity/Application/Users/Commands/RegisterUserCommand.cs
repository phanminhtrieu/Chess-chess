using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record RegisterUserCommand(string DisplayName, string Email, string Password) : ICommand<Guid>;

public class RegisterUserCommandHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly UserManager<User> _userManager;

    public RegisterUserCommandHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Guid> ExecuteAsync(RegisterUserCommand command, CancellationToken ct = default)
    {
        var user = new User
        {
            UserName = command.Email,
            Email = command.Email,
            DisplayName = command.DisplayName,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, command.Password);

        if (!result.Succeeded)
        {
            throw new Exception($"Failed to register user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user.Id;
    }
}
