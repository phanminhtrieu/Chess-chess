using ChessLearning.Modules.Learning.Infrastructure;
using ChessLearning.Modules.Learning.Application.Commands;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChessLearning.Modules.Learning;

public static class LearningModuleRegistration
{
    public static IServiceCollection AddLearningModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LearningDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register Application Handlers
        services.AddScoped<ICommandHandler<CreateLearningPlanCommand, Result<Guid>>, CreateLearningPlanCommandHandler>();

        return services;
    }
}
