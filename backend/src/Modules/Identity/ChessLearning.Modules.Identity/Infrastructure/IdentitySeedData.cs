using ChessLearning.Modules.Identity.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ChessLearning.Modules.Identity.Infrastructure;

public static class IdentitySeedData
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        string[] roles = { "Admin", "Teacher", "Student" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(role));
            }
        }

        // Seed Teacher
        await EnsureUserAsync(userManager, "teacher@chess.local", "Curator Ivory", "Teacher", "Test1234");
        
        // Seed Student
        await EnsureUserAsync(userManager, "student@chess.local", "Scholar Novak", "Student", "Test1234");

        // Seed Admin
        await EnsureUserAsync(userManager, "admin@chess.local", "Admin Root", "Admin", "Test1234");
    }

    private static async Task EnsureUserAsync(UserManager<User> userManager, string email, string displayName, string role, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
                Email = email,
                DisplayName = displayName,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}
