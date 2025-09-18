using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using LRA.Infrastructure.Azure.Interfaces;
using Microsoft.Extensions.Logging;

namespace LRA.Infrastructure.Azure.Implementations;

public class AzureBlobStorageService : IAzureBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly string _containerName = "images";
    private readonly ILogger<AzureBlobStorageService> _logger;
    
    public AzureBlobStorageService(BlobServiceClient blobServiceClient, ILogger<AzureBlobStorageService> logger)
    {
        _blobServiceClient = blobServiceClient;
        _logger = logger;
        InitializeContainerAsync().Wait();
    }
    
    private async Task InitializeContainerAsync()
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);
            _logger.LogInformation("Container {ContainerName} initialized", _containerName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize container");
            throw;
        }
    }
    
    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";
            
            var blobClient = containerClient.GetBlobClient(uniqueFileName);
            
            await blobClient.UploadAsync(
                imageStream, 
                new BlobHttpHeaders { ContentType = contentType });
            
            return blobClient.Name;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image");
            throw;
        }
    }
    
    public async Task<(Stream Content, string ContentType)> GetImageAsync(string blobName, CancellationToken cancellationToken)
    {
        try
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(_containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            if (!await blobClient.ExistsAsync(cancellationToken))
            {
                _logger.LogWarning("Blob {BlobName} not found", blobName);
                throw new FileNotFoundException($"Image {blobName} not found");
            }

            var response = await blobClient.DownloadStreamingAsync(cancellationToken: cancellationToken);
            return (response.Value.Content, response.Value.Details.ContentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image {BlobName}", blobName);
            throw;
        }
    }
}
