using System.Security.Claims;

namespace LRA.Common.Models;

public class JwtDecodeResult
{
    public ClaimsPrincipal? Principal { get; set; }
    public string? UserId { get; set; }
    public string? ClientId { get; set; }
    public string? Mail { get; set; }
}
