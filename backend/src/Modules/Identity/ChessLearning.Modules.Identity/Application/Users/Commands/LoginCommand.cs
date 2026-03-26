using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Modules.Identity.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChessLearning.Modules.Identity.Application.Users.Commands;

public record LoginCommand(string Email, string Password) : ICommand<LoginResultDto>;

public record LoginResultDto(string Token, string RefreshToken);

public class LoginCommandHandler : ICommandHandler<LoginCommand, LoginResultDto>
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _configuration;
    private readonly IdentityDbContext _db;

    public LoginCommandHandler(UserManager<User> userManager, SignInManager<User> signInManager, IConfiguration configuration, IdentityDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _configuration = configuration;
        _db = db;
    }

    public async Task<LoginResultDto> ExecuteAsync(LoginCommand command, CancellationToken ct = default)
    {
        var user = await _userManager.FindByEmailAsync(command.Email);
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid credentials or inactive user.");
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, command.Password, lockoutOnFailure: false);
        if (!result.Succeeded)
        {
            throw new UnauthorizedAccessException("Invalid credentials.");
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!)
        };

        if (!string.IsNullOrEmpty(role))
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Secret"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        _db.RefreshTokens.Add(refreshToken);
        await _db.SaveChangesAsync(ct);

        return new LoginResultDto(jwtToken, refreshToken.Token);
    }
}
