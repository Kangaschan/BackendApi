using LRA.Common.Models;

namespace LRA.Account.Domain.Models;

public class RoleEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public required string Name { get; set; }
    public ICollection<AccountEntity> Accounts { get; set; } = new List<AccountEntity>();
}
