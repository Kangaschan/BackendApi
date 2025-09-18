using LRA.Account.Application.DTOs;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;

namespace LRA.Account.Application.Interfaces;

public interface ITokenRepository
{
    Task<EmailConfirmationTokenDto> CheckToken(TokenDto dto, CancellationToken cancellationToken);
    Task<string> CreateToken(EmailConfirmationTokenDto emailConfirmationTokenDto, CancellationToken cancellationToken);
}
