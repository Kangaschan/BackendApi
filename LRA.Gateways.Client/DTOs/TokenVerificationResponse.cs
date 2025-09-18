namespace LRA.Gateways.Client.DTOs;

public class TokenVerificationResponse
{
    public bool Active { get; set; }
    public string? Subject { get; set; }
    public string? ClientId { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public string? Issuer { get; set; }
    public string? Error { get; set; }
}
