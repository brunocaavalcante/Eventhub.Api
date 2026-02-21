namespace Eventhub.Application.DTOs;

public class ImageStorageUploadRequest
{
    public string FileName { get; set; } = string.Empty;
    public byte[] Content { get; set; } = Array.Empty<byte>();
    public string? ContentType { get; set; }
    public string? FolderOverride { get; set; }
}

public class ImageStorageUploadResult
{
    public string PublicId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Bytes { get; set; }
}
