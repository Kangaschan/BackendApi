using Microsoft.AspNetCore.Http;

namespace LRA.Common.DTOs;

public class JobApplicationDto
{
    public required IFormFile IdentityDocumentPhoto { get; set; }
    public required IFormFile IdentityDocumentSelfie { get; set; }
    public required IFormFile MedicalCertificatePhoto { get; set; }
}
    
