using Eventhub.Application.DTOs;

namespace Eventhub.Application.Interfaces;

public interface IImageStorageService
{
    Task<ImageStorageUploadResult> UploadAsync(ImageStorageUploadRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(string publicId);
}
