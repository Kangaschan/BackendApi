using LRA.Common.Models;

namespace LRA.Account.Domain.Models;

public class CredentialsDetailsEntity : IBaseEntity
{
    public Guid Id { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }

    public bool IsUsed { get; set; }
    
    public bool IsTemporary { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    
    public Guid? AccountId { get; set; }
    
    public AccountEntity? Account { get; set; }
}
    
