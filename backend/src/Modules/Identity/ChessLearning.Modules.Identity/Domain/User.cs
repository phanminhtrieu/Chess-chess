using Microsoft.AspNetCore.Identity;
using ChessLearning.Shared.Domain;

namespace ChessLearning.Modules.Identity.Domain;

public class User : IdentityUser<Guid>
{
    public string? DisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    public void SetUpdatedAt() => UpdatedAt = DateTime.UtcNow;
}
