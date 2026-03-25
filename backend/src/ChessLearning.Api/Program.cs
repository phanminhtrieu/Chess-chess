using ChessLearning.Shared.Application.Contracts;
using ChessLearning.Shared.Infrastructure;
using ChessLearning.Modules.Identity;
using ChessLearning.Modules.Learning;
using ChessLearning.Modules.Assignment;
using ChessLearning.Modules.Content;
using ChessLearning.Modules.Game;
using ChessLearning.Modules.Progress;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Shared Services
builder.Services.AddSingleton<IChessEngineService, NullChessEngineService>();

// Register Modules
builder.Services
    .AddIdentityModule(builder.Configuration)
    .AddLearningModule(builder.Configuration)
    .AddAssignmentModule(builder.Configuration)
    .AddContentModule(builder.Configuration)
    .AddGameModule(builder.Configuration)
    .AddProgressModule(builder.Configuration);

// Add Hangfire
builder.Services.AddHangfire(configuration => configuration
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use Hangfire Dashboard (restrict in production!)
app.UseHangfireDashboard("/admin/hangfire");

app.UseAuthentication();
app.UseAuthorization();
app.UseAssignmentModule();

app.MapControllers();

app.Run();
