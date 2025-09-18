using LRA.Common.Enums;
using LRA.Common.Models;

namespace LRA.Common.DTOs.KYC;

public class JobApplication
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public KycStatusEnum Status { get; set; }
    public Guid AccountGuid { get; set; }
    public string? RejectReason { get; set; }
    public Guid? AdminReviewId { get; set; }
    public required FileDto IdentityDocumentPhoto { get; set; }
    public required FileDto IdentityDocumentSelfie { get; set; }
    public required FileDto MedicalCertificatePhoto { get; set; }
}
