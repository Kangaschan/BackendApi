using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static LRA.Common.Utils.TokenGerationUtils;

namespace LRA.Account.DBInfrastructure.Repositories;

public class TokenRepository : ITokenRepository
{
    
    private readonly IAppDbContext _context;

    public TokenRepository(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<EmailConfirmationTokenDto> CheckToken(TokenDto dto, CancellationToken cancellationToken)
    {
        var token = await _context.ConfirmationTokens
            .AsNoTracking()
            .Where(e => e.Token == dto.Token)
            .Select(entity => new EmailConfirmationTokenEntity
            {
                Id = entity.Id,
                Token = entity.Token,
                ExpiresAt = entity.ExpiresAt,
                UserEmail = entity.UserEmail,
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (token == null)
        {
            throw new KeyNotFoundException($"Email confirmation token with id {dto.Token} not found");
        }
        
        var res = new EmailConfirmationTokenDto
        {
            ExpiresAt = token.ExpiresAt,
            UserEmail = token.UserEmail,
            Token = token.Token,
        };
        
        if (token.ExpiresAt > DateTime.UtcNow)
        {
            var rowsDeleted = await _context.ConfirmationTokens
                .Where(t => t.Id == token.Id)
                .ExecuteUpdateAsync(setters => setters
                    .SetProperty(d=> d.IsUsed, true),
                cancellationToken);
        }
        return res;
    }
    
    public async Task<string> CreateToken(EmailConfirmationTokenDto emailConfirmationTokenDto,
        CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                var token = new EmailConfirmationTokenEntity
                {
                    ExpiresAt = emailConfirmationTokenDto.ExpiresAt,
                    Token = GenerateAlphanumericCode(8),
                    UserEmail = emailConfirmationTokenDto.UserEmail,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow
                };
                
                _context.ConfirmationTokens.Add(token);

                await _context.SaveChangesAsync();
                return token.Token;
            }
            catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
            {
            }
        }
    }
    
    public bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        return ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505";
    }
}
