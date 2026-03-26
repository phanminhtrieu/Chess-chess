using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Shared.Application.Contracts;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;

namespace ChessLearning.Modules.Identity.Application.Users.Queries;

public record GetProfileQuery() : IQuery<UserProfileDto>;

public class GetProfileQueryHandler : IQueryHandler<GetProfileQuery, UserProfileDto>
{
    private readonly UserManager<User> _userManager;
    private readonly ICurrentUserService _currentUser;

    public GetProfileQueryHandler(UserManager<User> userManager, ICurrentUserService currentUser)
    {
        _userManager = userManager;
        _currentUser = currentUser;
    }

    public async Task<UserProfileDto> HandleAsync(GetProfileQuery query, CancellationToken ct = default)
    {
        if (!_currentUser.IsAuthenticated)
            throw new UnauthorizedAccessException();

        var user = await _userManager.FindByIdAsync(_currentUser.UserId.ToString());
        if (user == null || !user.IsActive)
            throw new UnauthorizedAccessException();

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        return new UserProfileDto(user.Id.ToString(), user.DisplayName ?? string.Empty, user.Email!, role);
    }
}
