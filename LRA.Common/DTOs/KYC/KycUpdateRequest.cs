using LRA.Common.Enums;

namespace LRA.Common.DTOs.KYC;

public class KycUpdateRequest
{
    public KycStatusEnum Status { get; set; }
    public string? RejectReason { get; set; }
    public Guid? AdminReviewId { get; set; }    
}
