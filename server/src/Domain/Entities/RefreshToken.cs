using Domain.Entities.Common;

namespace Domain.Entities;

public class RefreshToken : BaseAuditableEntity
{
    public Guid UserId { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public virtual User User { get; set; } = default!;
}
