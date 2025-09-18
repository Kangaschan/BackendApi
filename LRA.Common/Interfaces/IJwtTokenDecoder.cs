using LRA.Common.Models;

namespace LRA.Common.Interfaces;

public interface IJwtTokenDecoder
{
    Task<JwtDecodeResult> DecodeTokenAsync(string token, CancellationToken cancellationToken);
}
