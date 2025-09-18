using LRA.Common.Models;

namespace LRA.Common.DTOs;

public class AccountDto
{
    public Guid Id { get; set; }
    public KeycloakId? KeycloakId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Phone { get; set; }
    public required string Email { get; set; }
    public ICollection<string>? Roles { get; set; }
    public bool IsBlocked { get; set; }
    public DateTime? BlockedUntil { get; set; }
    public bool IsTwoFactorEnabled  { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsTemporaryPassword { get; set; }
    public bool IsTemporaryPasswordUsed { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
