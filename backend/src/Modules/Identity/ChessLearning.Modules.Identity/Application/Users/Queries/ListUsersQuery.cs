using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ChessLearning.Modules.Identity.Application.Users.Queries;

public record ListUsersQuery() : IQuery<IEnumerable<UserListItemDto>>;

public class ListUsersQueryHandler : IQueryHandler<ListUsersQuery, IEnumerable<UserListItemDto>>
{
    private readonly UserManager<User> _userManager;

    public ListUsersQueryHandler(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<UserListItemDto>> HandleAsync(ListUsersQuery query, CancellationToken ct = default)
    {
        var list = new List<UserListItemDto>();
        var users = await _userManager.Users.ToListAsync(ct);
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Student";
            list.Add(new UserListItemDto(user.Id.ToString(), user.DisplayName ?? string.Empty, user.Email!, role, user.IsActive, user.CreatedAt, user.UpdatedAt));
        }

        return list;
    }
}
