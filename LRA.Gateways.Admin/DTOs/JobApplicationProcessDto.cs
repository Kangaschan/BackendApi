using LRA.Common.Enums;

namespace LRA.Gateways.Admin.DTOs;

public class JobApplicationProcessDto
{
    public Guid KycId { get; set; }
    public required KycStatusEnum Status { get; set; }
    public string? RejectReason { get; set; }
}
