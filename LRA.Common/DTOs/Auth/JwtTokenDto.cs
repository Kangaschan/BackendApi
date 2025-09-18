namespace LRA.Common.DTOs.Auth;

public class JwtTokenDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}
