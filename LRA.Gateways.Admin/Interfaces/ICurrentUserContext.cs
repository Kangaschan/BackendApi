namespace LRA.Gateways.Admin.Interfaces;

public interface ICurrentUserContext
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsSuperAdmin { get; }
}
