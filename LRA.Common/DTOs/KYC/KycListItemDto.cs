using LRA.Common.Enums;

namespace LRA.Common.DTOs.KYC;

public class KycListItemDto
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public KycStatusEnum Status { get; set; }
    public Guid AccountGuid { get; set; }
}
