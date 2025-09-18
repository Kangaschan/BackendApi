using Microsoft.AspNetCore.Http;

namespace LRA.Account.Application.Interfaces;

public interface IAccountImageService
{
    Task<string> UploadImageAsync(IFormFile image, CancellationToken cancellationToken);
    Task<(Stream Content, string ContentType)> GetImageAsync(string imageUrl, CancellationToken cancellationToken);
}
