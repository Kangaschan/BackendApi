namespace LRA.Infrastructure.Azure.Interfaces;

public interface IAzureBlobStorageService
{
    Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken);
    Task<(Stream Content, string ContentType)> GetImageAsync(string blobName, CancellationToken cancellationToken);
}
