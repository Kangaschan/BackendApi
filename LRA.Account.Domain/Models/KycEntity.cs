using LRA.Common.Enums;
using LRA.Common.Models;

namespace LRA.Account.Domain.Models;

public class KycEntity : IBaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }
    public required string IdentityDocumentPhoto { get; set; }
    public required string IdentityDocumentSelfie { get; set; }
    public required string MedicalCertificatePhoto { get; set; }
    public required Guid AccountId { get; set; }
    public Guid? AdminReviewId { get; set; }
    public string? RejectReason { get; set; }
    public KycStatusEnum Status { get; set; }
}
