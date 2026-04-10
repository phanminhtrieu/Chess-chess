using ChessLearning.Shared.Application.Contracts;
using ChessLearning.Shared.Infrastructure;
using ChessLearning.Modules.Identity;
using ChessLearning.Modules.Learning;
using ChessLearning.Modules.Assignment;
using ChessLearning.Modules.Content;
using ChessLearning.Modules.Game;
using ChessLearning.Modules.Progress;
using Hangfire;
using Microsoft.EntityFrameworkCore; // Thêm dòng này để dùng được lệnh Migrate

var builder = WebApplication.CreateBuilder(args);

// Add basic services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Shared Services
builder.Services.AddSingleton<IChessEngineService, NullChessEngineService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, ChessLearning.Api.Services.CurrentUserService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", builder =>
    {
        builder.WithOrigins("http://localhost:4200", "http://localhost:5000")
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

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

// Khởi tạo Database và Seed Identity Data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try 
    {
        // 1. Tự động tạo DB và chạy Migration
        var context = services.GetRequiredService<ChessLearning.Modules.Identity.Infrastructure.IdentityDbContext>();
        await context.Database.MigrateAsync();

        // 2. Thực hiện Seed dữ liệu
        await ChessLearning.Modules.Identity.Infrastructure.IdentitySeedData.SeedAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Đã xảy ra lỗi trong quá trình khởi tạo Database hoặc Seed dữ liệu.");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAngular");

// app.UseHttpsRedirection();

// Use Hangfire Dashboard (restrict in production!)
app.UseHangfireDashboard("/admin/hangfire");

app.UseAuthentication();
app.UseAuthorization();
app.UseAssignmentModule();

app.MapControllers();

app.Run();
