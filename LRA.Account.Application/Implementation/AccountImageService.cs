using LRA.Account.Application.Interfaces;
using LRA.Infrastructure.Azure.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LRA.Account.Application.Implementation;

public class AccountImageService: IAccountImageService
{
    private readonly IAzureBlobStorageService _azureBlobStorageService;

    public AccountImageService(IAzureBlobStorageService azureBlobStorageService)
    {
        _azureBlobStorageService = azureBlobStorageService;
    }
    
    public async Task<string> UploadImageAsync(IFormFile image, CancellationToken cancellationToken)
    {
        var imageUrl = await _azureBlobStorageService.UploadImageAsync(image.OpenReadStream(), image.FileName, image.ContentType, cancellationToken);
        return imageUrl;
    }

    public async Task<(Stream Content, string ContentType)> GetImageAsync(string imageUrl, CancellationToken cancellationToken)
    {
        var image = await _azureBlobStorageService.GetImageAsync(imageUrl, cancellationToken);
        return image;
    }
}
