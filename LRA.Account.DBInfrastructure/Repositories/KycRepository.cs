using LRA.Account.Application.DTOs;
using LRA.Account.Application.Interfaces;
using LRA.Account.DBInfrastructure.Data;
using LRA.Account.Domain.Models;
using LRA.Common.DTOs.KYC;
using LRA.Common.Enums;
using Microsoft.EntityFrameworkCore;

namespace LRA.Account.DBInfrastructure.Repositories;

public class KycRepository : IKycRepository
{
    private readonly IAppDbContext _context;

    public KycRepository(IAppDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(KycCreateRequest request,CancellationToken cancellationToken)
    {
        var kycEntity = new KycEntity
        {
            AccountId = request.AccountId,
            IdentityDocumentPhoto = request.IdentityDocumentPhoto,
            IdentityDocumentSelfie = request.IdentityDocumentSelfie,
            MedicalCertificatePhoto = request.MedicalCertificatePhoto,
            CreatedAtUtc = DateTime.UtcNow,
            UpdatedAtUtc = DateTime.UtcNow,
            Status = KycStatusEnum.Pending
        };
        
        _context.Kycs.Add(kycEntity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<KycListItemDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        if(!cancellationToken.IsCancellationRequested)
        {
            var kycList = _context.Kycs.AsNoTracking()
                .Select(kyc => new KycListItemDto
                    {
                        Id = kyc.Id,
                        AccountGuid = kyc.AccountId,
                        CreatedAt = kyc.UpdatedAtUtc,
                        Status = kyc.Status,
                    }
                );
            return kycList;
        }
        else
        {
            return null;
        }
    }

    public async Task<KycDetailsDto> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var kyc = await _context.Kycs.AsNoTracking().Where(kyc => kyc.Id == id).Select(kyc => new KycDetailsDto
        {
            Id = kyc.Id,
            CreatedAt = kyc.CreatedAtUtc,
            UpdatedAt = kyc.UpdatedAtUtc,
            Status = kyc.Status,
            AccountGuid = kyc.AccountId,
            IdentityDocumentPhoto = kyc.IdentityDocumentPhoto,
            IdentityDocumentSelfie = kyc.IdentityDocumentSelfie,
            MedicalCertificatePhoto = kyc.MedicalCertificatePhoto,
            AdminReviewId = kyc.AdminReviewId,
            RejectReason = kyc.RejectReason,
        }).FirstOrDefaultAsync(cancellationToken);

        if (kyc == null)
        {
            throw new KeyNotFoundException($"Kyc with id {id} not found");
        }
        
        return kyc;
    }

    public async Task<IEnumerable<KycListItemDto>> GetByAccountIdAsync(Guid accountId, CancellationToken cancellationToken)
    {
        if(!cancellationToken.IsCancellationRequested)
        {
            var kycList = _context.Kycs.AsNoTracking()
                .Where(hyc => hyc.AccountId == accountId)
                .Select(kyc => new KycListItemDto
                    {
                        Id = kyc.Id,
                        AccountGuid = kyc.AccountId,
                        CreatedAt = kyc.UpdatedAtUtc,
                        Status = kyc.Status,
                    }
                );
            return kycList;
        }
        else
        {
            return null;
        }
    }

    public async Task UpdateAsync(Guid id, KycUpdateRequest request, CancellationToken cancellationToken)
    {
        var rowsUpdated = await _context.Kycs.Where(kyc => kyc.Id == id)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(kyc => kyc.Status, request.Status)
                .SetProperty(kyc => kyc.RejectReason, request.RejectReason)
                .SetProperty(kyc => kyc.AdminReviewId, request.AdminReviewId)
                .SetProperty(kyc => kyc.UpdatedAtUtc, DateTime.UtcNow), 
                cancellationToken);
        if (rowsUpdated == 0)
        {
            throw new KeyNotFoundException($"Kyc with id {id} not found");
        }
        
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var rowsDeleted = await _context.Kycs.Where(kyc => kyc.Id == id).ExecuteDeleteAsync(cancellationToken);
        
        if (rowsDeleted == 0)
        {
            throw new KeyNotFoundException($"Kyc with id {id} not found");
        }
    }
}
