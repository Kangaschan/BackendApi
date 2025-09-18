using LRA.Common.Models;

namespace LRA.Account.Domain.Models;

public class OneTimePasswordEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public required string Password { get; set; }
    public required string UserEmail { get; set; }
    public DateTime ExpiresAt { get; set; }
}
