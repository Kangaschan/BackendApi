using LRA.Common.Enums;

namespace LRA.Common.DTOs.KYC;

public class KycDetailsDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public KycStatusEnum Status { get; set; }
    public Guid AccountGuid { get; set; }
    public string? RejectReason { get; set; }
    public Guid? AdminReviewId { get; set; }    
    public required string IdentityDocumentPhoto { get; set; }
    public required string IdentityDocumentSelfie { get; set; }
    public required string MedicalCertificatePhoto { get; set; }
}
