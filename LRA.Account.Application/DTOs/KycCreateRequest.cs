namespace LRA.Account.Application.DTOs;

public class KycCreateRequest
{
    public required string IdentityDocumentPhoto  { get; set; }
    public required string IdentityDocumentSelfie  { get; set; }
    public required string MedicalCertificatePhoto  { get; set; }
    public required Guid AccountId  { get; set; }
}
