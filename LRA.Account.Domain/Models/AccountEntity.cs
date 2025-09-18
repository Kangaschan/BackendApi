using LRA.Common.Models;

namespace LRA.Account.Domain.Models;

public class AccountEntity : IBaseEntity, ISoftDeletableEntity
{
    public Guid Id { get; set; }
    
    public required string KeycloakId { get; set; }
    
    public string? FirstName { get; set; }
    
    public string? LastName { get; set; }

    public required string Email { get; set; }
    
    public string? Phone { get; set; }
    
    public bool IsDeleted { get; set; }
    
    public DateTime CreatedAtUtc { get; set; }
    
    public DateTime UpdatedAtUtc { get; set; }
    
    public ICollection<RoleEntity>? Roles { get; set; } = new List<RoleEntity>();
    
    public required bool IsBlocked { get; set; }
    
    public DateTime? BlockedUntil { get; set; }
    
    public required bool IsTwoFactorEnabled  { get; set; }
        
    public CredentialsDetailsEntity? Credentials { get; set; }
    
    public ICollection<KycEntity>? Kycs { get; set; } = new List<KycEntity>();
}
