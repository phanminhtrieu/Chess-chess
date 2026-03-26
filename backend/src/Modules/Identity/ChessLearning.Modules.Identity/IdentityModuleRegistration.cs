using ChessLearning.Modules.Identity.Domain;
using ChessLearning.Modules.Identity.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessLearning.Modules.Identity;

public static class IdentityModuleRegistration
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddIdentity<User, IdentityRole<Guid>>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 4;
        })
            .AddEntityFrameworkStores<IdentityDbContext>()
            .AddDefaultTokenProviders();

        var jwtSettings = configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["Secret"];

        if (!string.IsNullOrEmpty(secretKey))
        {
            var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(secretKey));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = key
                };
            });
        }

        // Register Handlers
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.RegisterUserCommand, Guid>, Application.Users.Commands.RegisterUserCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.LoginCommand, Application.Users.Commands.LoginResultDto>, Application.Users.Commands.LoginCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.RefreshSessionCommand, Application.Users.Commands.LoginResultDto>, Application.Users.Commands.RefreshSessionCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.LogoutCommand>, Application.Users.Commands.LogoutCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.IQueryHandler<Application.Users.Queries.GetProfileQuery, Application.Users.UserProfileDto>, Application.Users.Queries.GetProfileQueryHandler>();
        
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.UpdateUserCommand>, Application.Users.Commands.UpdateUserCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.RevokeUserSessionsCommand>, Application.Users.Commands.RevokeUserSessionsCommandHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.ICommandHandler<Application.Users.Commands.DeactivateUserCommand>, Application.Users.Commands.DeactivateUserCommandHandler>();
        
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.IQueryHandler<Application.Users.Queries.ListUsersQuery, IEnumerable<Application.Users.UserListItemDto>>, Application.Users.Queries.ListUsersQueryHandler>();
        services.AddTransient<ChessLearning.Shared.Application.Contracts.Cqrs.IQueryHandler<Application.Users.Queries.GetUserAuditQuery, Application.Users.UserAuditDto>, Application.Users.Queries.GetUserAuditQueryHandler>();

        return services;
    }
}
