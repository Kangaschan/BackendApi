using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using static LRA.Common.Utils.TokenGerationUtils;

namespace LRA.Account.DBInfrastructure.Repositories;

public class OneTimePasswordRepository : IOneTimePasswordRepository
{
    private readonly IAppDbContext _context;

    public OneTimePasswordRepository(IAppDbContext context)
    {
        _context = context;
    }
    
    public async Task<OneTimePasswordDto> CheckPassword(TokenDto dto, CancellationToken cancellationToken)
    {
        var password = await _context.OneTinePasswords
            .AsNoTracking()
            .Where(e => e.Password == dto.Token)
            .Select(entity => new OneTimePasswordEntity
            {
                Id = entity.Id,
                Password = entity.Password,
                ExpiresAt = entity.ExpiresAt,
                UserEmail = entity.UserEmail,
            }).FirstOrDefaultAsync(cancellationToken);
        
        if (password == null)
        {
            throw new KeyNotFoundException($"One time password {dto.Token} not found");
        }
        
        var res = new OneTimePasswordDto
        {
            ExpiresAt = password.ExpiresAt,
            UserEmail = password.UserEmail,
            Password = password.Password,
        };
        
        if (password.ExpiresAt > DateTime.UtcNow)
        {
            var rowsDeleted = await _context.OneTinePasswords
                .Where(t => t.Id == password.Id)
                .ExecuteDeleteAsync(cancellationToken);
        }
        return res;
    }

    public async Task<string> CreatePassword(OneTimePasswordDto oneTimePasswordDto, CancellationToken cancellationToken)
    {
        while (true)
        {
            try
            {
                var password = new OneTimePasswordEntity()
                {
                    ExpiresAt = oneTimePasswordDto.ExpiresAt,
                    Password = GenerateAlphanumericCode(8),
                    UserEmail = oneTimePasswordDto.UserEmail,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                };
                
                _context.OneTinePasswords.Add(password);

                await _context.SaveChangesAsync();
                return password.Password;
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
