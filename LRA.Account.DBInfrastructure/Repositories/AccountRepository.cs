using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs;
using LRA.Common.DTOs.Auth;
using LRA.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace LRA.Account.DBInfrastructure.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly IAppDbContext _context;

    public AccountRepository(IAppDbContext context)
    {
        _context = context;
    }
    
    private async Task<ICollection<RoleEntity>> ParseToRoles(ICollection<string> roles, CancellationToken cancellationToken)
    {
        var result = await _context.Roles.Where(role => roles.Contains(role.Name)).ToListAsync(cancellationToken);
        return result;
    }

    private static ICollection<string> ParseFromRoles(ICollection<RoleEntity> roles)
    {
        var result = new List<string>();
        foreach (var role in roles)
        {
            result.Add(role.Name);
        }
        return result;
    }
    
    public async Task<IEnumerable<AccountDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var accounts = _context.Accounts.AsNoTracking()
            .Include(a=>a.Credentials)
            .Select(account => new AccountDto
        {
            Id = account.Id,
            LastName = account.LastName,
            KeycloakId = new KeycloakId{Key = account.KeycloakId},
            FirstName = account.FirstName,
            Email = account.Email,
            Phone = account.Phone,
            CreatedAt = account.CreatedAtUtc,
            UpdatedAt = account.UpdatedAtUtc,
            IsBlocked = account.IsBlocked,
            BlockedUntil = account.BlockedUntil,
            Roles = ParseFromRoles(account.Roles),
            IsTwoFactorEnabled = account.IsTwoFactorEnabled,
            IsTemporaryPasswordUsed = account.Credentials.IsUsed,
            IsTemporaryPassword = account.Credentials.IsTemporary,
            ExpiresAt = account.Credentials.ExpiresAt,  
        });
        return accounts;
    }

    public async Task<AccountDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .Include(a=>a.Credentials)
            .Where(account => account.Id == id)
            .Select(account => new AccountDto
            {
                Id = account.Id,
                LastName = account.LastName,
                KeycloakId = new KeycloakId{Key = account.KeycloakId},
                FirstName = account.FirstName,
                Email = account.Email,
                Phone = account.Phone,
                CreatedAt = account.CreatedAtUtc,
                UpdatedAt = account.UpdatedAtUtc,
                IsBlocked = account.IsBlocked,
                BlockedUntil = account.BlockedUntil,
                Roles = ParseFromRoles(account.Roles),
                IsTwoFactorEnabled = account.IsTwoFactorEnabled,
                IsTemporaryPasswordUsed = account.Credentials.IsUsed,
                IsTemporaryPassword = account.Credentials.IsTemporary,
                ExpiresAt = account.Credentials.ExpiresAt,  
            })
            .FirstOrDefaultAsync(cancellationToken);
        return account;
    }
    
    public async Task<AccountDto?> GetByKeycloakIdAsync(KeycloakId id, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .Include(a=>a.Credentials)
            .Where(account => account.KeycloakId == id.Key)
            .Select(account => new AccountDto
            {
                Id = account.Id,
                LastName = account.LastName,
                KeycloakId = new KeycloakId{Key = account.KeycloakId},
                FirstName = account.FirstName,
                Email = account.Email,
                Phone = account.Phone,
                CreatedAt = account.CreatedAtUtc,
                UpdatedAt = account.UpdatedAtUtc,
                IsBlocked = account.IsBlocked,
                BlockedUntil = account.BlockedUntil,
                Roles = ParseFromRoles(account.Roles),
                IsTwoFactorEnabled = account.IsTwoFactorEnabled,
                IsTemporaryPasswordUsed = account.Credentials.IsUsed,
                IsTemporaryPassword = account.Credentials.IsTemporary,
                ExpiresAt = account.Credentials.ExpiresAt,  
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return account;
    }

    public async Task<PagedResult<AccountSearchResult>> GetFilteredAsync(ViewFilterParams filterParams, CancellationToken cancellationToken)
    {
        var query = _context.Accounts
        .AsNoTracking()
        .Include(a=>a.Credentials)
        .Include(a=>a.Roles)
        .AsQueryable();

        if (filterParams.IncludeAdmins.HasValue  || filterParams.IncludeClients.HasValue || filterParams.IncludeParamedics.HasValue)
        {
            query = query.Where(a =>
                (filterParams.IncludeAdmins == true && a.Roles.Any(r => r.Name == "admin" || r.Name == "superAdmin")) ||
                (filterParams.IncludeClients == true && a.Roles.Any((r => r.Name == "client")) || 
                (filterParams.IncludeParamedics == true && a.Roles.Any(r => r.Name == "paramedic")) ||
                (filterParams.IncludeAdmins == false && filterParams.IncludeClients == false && filterParams.IncludeParamedics == false)
                ));
        }
        if (filterParams.IncludeBlocked.HasValue)
        {
            query = query.Where(a => a.IsBlocked == filterParams.IncludeBlocked);
        }
        var projectedQuery = query.Select(account => new AccountSearchResult
            {
                Id = account.Id,
                Email = account.Email,
                CreatedAt = account.CreatedAtUtc,
                UpdatedAt = account.UpdatedAtUtc,
                IsBlocked = account.IsBlocked,
                Roles = ParseFromRoles(account.Roles),
                IsDeleted = account.IsDeleted,
            });
        
        var totalCount = await projectedQuery.CountAsync(cancellationToken);
        
        var items = await projectedQuery
            .OrderBy(dto => dto.Id)
            .Skip((filterParams.PageNumber - 1) * filterParams.PageSize)
            .Take(filterParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AccountSearchResult>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filterParams.PageNumber,
            PageSize = filterParams.PageSize
        };
    }

    public async Task CreateAsync(AccountDto account, CancellationToken cancellationToken)
    {
        var accountEntity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            LastName = account.LastName,
            KeycloakId = account.KeycloakId.Key,
            FirstName = account.FirstName,
            Email = account.Email,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            Roles = await ParseToRoles(account.Roles, cancellationToken),
            Phone = account.Phone,
            IsBlocked = false,
            IsTwoFactorEnabled = true
        };

        var credentials = new CredentialsDetailsEntity
        {
            Id = Guid.NewGuid(),
            AccountId = accountEntity.Id,
            IsUsed = account.IsTemporaryPasswordUsed,
            IsTemporary = account.IsTemporaryPassword,
            ExpiresAt = account.ExpiresAt,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow
        };
        accountEntity.Credentials = credentials;
        _context.Accounts.Add(accountEntity);
        _context.CredentialsDetails.Add(credentials);

        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Guid id, AccountDto account, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _context.Accounts
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.Email, account.Email)
                .SetProperty(a => a.KeycloakId, account.KeycloakId.Key)
                .SetProperty(a => a.FirstName, account.FirstName)
                .SetProperty(a => a.LastName, account.LastName)
                .SetProperty(a => a.Phone, account.Phone)
                .SetProperty(a => a.IsBlocked, account.IsBlocked)
                .SetProperty(a => a.BlockedUntil, account.BlockedUntil)
                .SetProperty(a => a.UpdatedAtUtc, DateTime.UtcNow));

        if (rowsUpdated == 0)
        {
            throw new KeyNotFoundException($"Account with id {id} not found");
        }

        var accountEntity = await _context.Accounts
            .Include(a => a.Roles)
            .Include(a => a.Credentials)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (accountEntity != null)
        {
            var roles = await ParseToRoles(account.Roles, cancellationToken);
            accountEntity.Roles.Clear();
            foreach (var role in roles)
            {
                accountEntity.Roles.Add(role);
            }

            accountEntity.Credentials.IsTemporary = account.IsTemporaryPassword;
            accountEntity.Credentials.IsUsed = account.IsTemporaryPasswordUsed;
            accountEntity.Credentials.UpdatedAtUtc = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _context.Accounts
            .Where(a => a.Id == id)
            .ExecuteDeleteAsync();

        if (rowsDeleted == 0)
        {
            throw new KeyNotFoundException($"Account with id {id} not found");
        }
    }

    public async Task SoftDeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _context.Accounts
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsDeleted, true));
        
        if (rowsDeleted == 0)
        {
            throw new KeyNotFoundException($"Account with id {id} not found");
        }
    }

    public async Task<AccountDto?> GetbyEmail(string email, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .Include(a=>a.Credentials)
            .Where(account => account.Email == email)
            .Select(account => new AccountDto
            {
                Id = account.Id,
                LastName = account.LastName,
                KeycloakId = new KeycloakId{Key = account.KeycloakId},
                FirstName = account.FirstName,
                Email = account.Email,
                Phone = account.Phone,
                CreatedAt = account.CreatedAtUtc,
                UpdatedAt = account.UpdatedAtUtc,
                Roles = ParseFromRoles(account.Roles),
                IsTwoFactorEnabled = account.IsTwoFactorEnabled,
                IsTemporaryPasswordUsed = account.Credentials.IsUsed,
                IsTemporaryPassword = account.Credentials.IsTemporary,
                ExpiresAt = account.Credentials.ExpiresAt,  
            })
            .FirstOrDefaultAsync(cancellationToken);
        return account;
    }

    public async Task<PagedResult<AccountSearchResult>> SearchAsync(AccountSearchParams searchParams, CancellationToken cancellationToken)
    {
        var query = _context.Accounts
            .AsNoTracking()
            .Include(a=>a.Credentials)
            .Include(a=>a.Roles)
            .AsQueryable();

        if (searchParams.IncludeAdmins.HasValue  || searchParams.IncludeClients.HasValue || searchParams.IncludeParamedics.HasValue)
        {
            query = query.Where(a =>
                (searchParams.IncludeAdmins == true && a.Roles.Any(r => r.Name == "admin" || r.Name == "superAdmin")) ||
                (searchParams.IncludeClients == true && a.Roles.Any((r => r.Name == "client")) ||
                 (searchParams.IncludeParamedics == true && a.Roles.Any(r => r.Name == "paramedic")) ||
                 (searchParams.IncludeAdmins == false && searchParams.IncludeClients == false &&
                  searchParams.IncludeParamedics == false)
                ));
        }
        if (searchParams.IncludeBlocked.HasValue)
        {
            query = query.Where(a => a.IsBlocked == searchParams.IncludeBlocked);
        }

        if (!string.IsNullOrWhiteSpace(searchParams.EmailSearch))
        {
            query = query.Where(a => 
                a.Email.Contains(searchParams.EmailSearch));
        }
        if (!string.IsNullOrWhiteSpace(searchParams.FirstNameSearch))
        {
            query = query.Where(a =>  
                a.FirstName!=null &&
                a.FirstName.Contains(searchParams.FirstNameSearch));
        }
        if (!string.IsNullOrWhiteSpace(searchParams.LastNameSearch))
        {
            query = query.Where(a => 
                a.LastName!=null &&
                a.LastName.Contains(searchParams.LastNameSearch));
        }
        if (!string.IsNullOrWhiteSpace(searchParams.PhoneSearch))
        {
            query = query.Where(a =>  
                a.Phone!=null &&
                a.Phone.Contains(searchParams.PhoneSearch));
        }

        var projectedQuery = query.Select(account => new AccountSearchResult
        {
            Id = account.Id,
            Email = account.Email,
            CreatedAt = account.CreatedAtUtc,
            UpdatedAt = account.CreatedAtUtc,
            IsBlocked = account.IsBlocked,
            Roles = ParseFromRoles(account.Roles),
            IsDeleted = account.IsDeleted,
        });

        var totalCount = await projectedQuery.CountAsync(cancellationToken);
        
        var items = await projectedQuery
            .OrderBy(dto => dto.Id)
            .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
            .Take(searchParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<AccountSearchResult>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = searchParams.PageNumber,
            PageSize = searchParams.PageSize
        };
    }
    
    public async Task Update2FaAsync(KeycloakId keycloakId, Change2FaRequest change2FaRequest, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _context.Accounts
            .Where(u => u.KeycloakId == keycloakId.Key)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.IsTwoFactorEnabled, change2FaRequest.IsTwoFactorEnabled)
                .SetProperty(a => a.UpdatedAtUtc, DateTime.UtcNow));
        if (rowsUpdated == 0)
        {
            throw new KeyNotFoundException($"Account with keycloak id {keycloakId.Key} not found");
        }
    }

    public async Task ChangeFullNameAsync(KeycloakId keycloakId, ChangeFullNameRequest changeFullNameRequest, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _context.Accounts
            .Where(u => u.KeycloakId == keycloakId.Key)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.FirstName, changeFullNameRequest.FirstName)
                .SetProperty(a => a.LastName, changeFullNameRequest.LastName)
                .SetProperty(a => a.UpdatedAtUtc, DateTime.UtcNow));
        if (rowsUpdated == 0)
        {
            throw new KeyNotFoundException($"Account with keycloak id {keycloakId.Key} not found");
        }   
    }

    public async Task UpdateCredentialsAsync(string email, UpdateAccountCredentialsRequest request, CancellationToken cancellationToken)
    {
        var account = await _context.Accounts
            .Include(a => a.Credentials)
            .FirstOrDefaultAsync(a => a.Email == email, cancellationToken);

        if (account == null) 
            throw new KeyNotFoundException($"Account with email {email} not found");

        if (account.Credentials == null)
        {
            account.Credentials = new CredentialsDetailsEntity
            {
                IsUsed = request.IsTemporaryPasswordUsed,
                IsTemporary = request.IsTemporaryPassword,
                ExpiresAt = request.ExpiresAt,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            };
        }
        else
        {
            account.Credentials.IsUsed = request.IsTemporaryPasswordUsed;
            account.Credentials.IsTemporary = request.IsTemporaryPassword;
            account.Credentials.ExpiresAt = request.ExpiresAt;
            account.Credentials.UpdatedAtUtc = DateTime.UtcNow;
        }

        account.UpdatedAtUtc = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BlockAsync(Guid id, BlockRequest request, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _context.Accounts
            .Where(u => u.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(a => a.IsBlocked, request.IsBlocked)
                .SetProperty(a => a.BlockedUntil, request.BlockedUntil)
                .SetProperty(a => a.UpdatedAtUtc, DateTime.UtcNow));

        if (rowsUpdated == 0)
        {
            throw new KeyNotFoundException($"Account with id {id} not found");
        }
    }
}
