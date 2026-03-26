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

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole<Guid>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ChessLearning.Modules.Identity.Domain.User>>();
    
    string[] roles = { "Student", "Teacher", "Admin" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole<Guid>(role));
        }
    }

    var adminEmail = "admin@gmail.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new ChessLearning.Modules.Identity.Domain.User
        {
            UserName = adminEmail,
            Email = adminEmail,
            DisplayName = "Administrator",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            EmailConfirmed = true
        };
        await userManager.CreateAsync(adminUser, "123456");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Use Hangfire Dashboard (restrict in production!)
app.UseHangfireDashboard("/admin/hangfire");

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();
app.UseAssignmentModule();

app.MapControllers();

app.Run();
