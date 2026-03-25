using ChessLearning.Modules.Assignment.Application.Assignments;
using ChessLearning.Modules.Assignment.Application.Assignments.Commands;
using ChessLearning.Modules.Assignment.Application.Assignments.Queries;
using ChessLearning.Modules.Assignment.Application.Assignments.Jobs;
using ChessLearning.Modules.Assignment.Infrastructure;
using ChessLearning.Shared.Application.Contracts.Cqrs;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Domain = ChessLearning.Modules.Assignment.Domain;

namespace ChessLearning.Modules.Assignment;

public static class AssignmentModuleRegistration
{
    public static IServiceCollection AddAssignmentModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AssignmentDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Register Command Handlers
        services.AddScoped<ICommandHandler<CreateAssignmentCommand, Guid>, CreateAssignmentCommandHandler>();
        services.AddScoped<ICommandHandler<AddTaskToAssignmentCommand>, AddTaskToAssignmentCommandHandler>();
        services.AddScoped<ICommandHandler<ActivateAssignmentCommand>, ActivateAssignmentCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteTaskCommand>, CompleteTaskCommandHandler>();
        services.AddScoped<ICommandHandler<CompleteAssignmentCommand>, CompleteAssignmentCommandHandler>();
        services.AddScoped<ICommandHandler<MarkAssignmentOverdueCommand>, MarkAssignmentOverdueCommandHandler>();
        services.AddScoped<ICommandHandler<FinalizeAssignmentCommand>, FinalizeAssignmentCommandHandler>();
        services.AddScoped<ICommandHandler<HandleOverdueWorkCommand>, HandleOverdueWorkCommandHandler>();

        // Register Query Handlers
        services.AddScoped<IQueryHandler<GetAssignmentDetailQuery, AssignmentDto?>, GetAssignmentDetailQueryHandler>();
        services.AddScoped<IQueryHandler<ListAssignmentsQuery, IEnumerable<AssignmentDto>>, ListAssignmentsQueryHandler>();
        services.AddScoped<IQueryHandler<AssignmentDashboardQuery, AssignmentDashboardDto>, AssignmentDashboardQueryHandler>();
        services.AddScoped<IQueryHandler<TaskListInspectionQuery, IEnumerable<TaskInspectionDto>>, TaskListInspectionQueryHandler>();
        services.AddScoped<IQueryHandler<ContentCorrelationViewQuery, IEnumerable<ContentCorrelationDto>>, ContentCorrelationViewQueryHandler>();

        // Register Jobs
        services.AddScoped<OverdueMonitorJob>();

        return services;
    }

    public static IApplicationBuilder UseAssignmentModule(this IApplicationBuilder app)
    {
        using (var scope = app.ApplicationServices.CreateScope())
        {
            var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
            recurringJobManager.AddOrUpdate<OverdueMonitorJob>(
                "overdue-monitor",
                job => job.ExecuteAsync(),
                Cron.Hourly);
        }

        return app;
    }
}
