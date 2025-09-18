using StackExchange.Redis;

namespace LRA.Common.DTOs.Auth;

public class AccountSearchResult
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsBlocked { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<string> Roles { get; set; }
}
