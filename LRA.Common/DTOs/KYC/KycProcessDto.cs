using LRA.Common.Enums;

namespace LRA.Common.DTOs.KYC;

public class KycProcessDto
{
    public Guid KycId { get; set; }
    public required string AdminEmail { get; set; }
    public required KycStatusEnum Status { get; set; }
    public string? RejectReason { get; set; }
}
