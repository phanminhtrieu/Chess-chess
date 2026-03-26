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

public record RefreshSessionCommand(string RefreshToken) : ICommand<LoginResultDto>;

public class RefreshSessionCommandHandler : ICommandHandler<RefreshSessionCommand, LoginResultDto>
{
    private readonly IdentityDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _configuration;

    public RefreshSessionCommandHandler(IdentityDbContext db, UserManager<User> userManager, IConfiguration configuration)
    {
        _db = db;
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<LoginResultDto> ExecuteAsync(RefreshSessionCommand command, CancellationToken ct = default)
    {
        var activeToken = await _db.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == command.RefreshToken, ct);

        if (activeToken == null || !activeToken.IsActive)
        {
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");
        }

        var user = await _userManager.FindByIdAsync(activeToken.UserId.ToString());
        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException("User is inactive or deleted.");
        }

        // Revoke the old token
        activeToken.RevokedAt = DateTime.UtcNow;

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? "Student";

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Role, role)
        };

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

        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        _db.RefreshTokens.Add(newRefreshToken);
        await _db.SaveChangesAsync(ct);

        return new LoginResultDto(jwtToken, newRefreshToken.Token);
    }
}
