using LRA.Account.Application.DTOs;
using LRA.Common.DTOs;

namespace LRA.Account.Application.Interfaces;

public interface IOneTimePasswordRepository
{
    Task<OneTimePasswordDto> CheckPassword(TokenDto dto, CancellationToken cancellationToken);
    Task<string> CreatePassword(OneTimePasswordDto oneTimePasswordDto, CancellationToken cancellationToken);
}
