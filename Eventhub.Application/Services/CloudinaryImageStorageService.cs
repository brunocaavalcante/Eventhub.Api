using System;
using System.Net;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Eventhub.Application.DTOs;
using Eventhub.Application.Interfaces;
using Eventhub.Application.Settings;
using Microsoft.Extensions.Options;

namespace Eventhub.Application.Services;

public class CloudinaryImageStorageService : IImageStorageService
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinarySettings _settings;

    public CloudinaryImageStorageService(IOptions<CloudinarySettings> options)
    {
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));

        if (string.IsNullOrWhiteSpace(_settings.CloudName) ||
            string.IsNullOrWhiteSpace(_settings.ApiKey) ||
            string.IsNullOrWhiteSpace(_settings.ApiSecret))
        {
            throw new InvalidOperationException("Configurações do Cloudinary não foram definidas corretamente.");
        }

        var account = new Account(_settings.CloudName, _settings.ApiKey, _settings.ApiSecret);
        _cloudinary = new Cloudinary(account);
        _cloudinary.Api.Secure = true;
    }

    public async Task<ImageStorageUploadResult> UploadAsync(ImageStorageUploadRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Content == null || request.Content.Length == 0)
            throw new ArgumentException("Conteúdo da imagem é obrigatório.", nameof(request));

        await using var stream = new MemoryStream(request.Content);
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(request.FileName, stream),
            Folder = !string.IsNullOrWhiteSpace(request.FolderOverride) ? request.FolderOverride : _settings.Folder,
            UseFilename = true,
            UniqueFilename = true,
            Overwrite = false
        };

        var result = await _cloudinary.UploadAsync(uploadParams, cancellationToken);

        if (result.Error != null ||
            (result.StatusCode != HttpStatusCode.OK && result.StatusCode != HttpStatusCode.Created))
        {
            var message = result.Error?.Message ?? "Falha ao enviar imagem ao Cloudinary.";
            throw new InvalidOperationException(message);
        }

        return new ImageStorageUploadResult
        {
            PublicId = result.PublicId,
            Url = ResolveUrl(result),
            ContentType = ResolveContentType(request.ContentType, result.Format),
            Bytes = result.Bytes,
        };
    }

    public async Task DeleteAsync(string publicId)
    {
        if (string.IsNullOrWhiteSpace(publicId))
            return;

        var deletionParams = new DeletionParams(publicId)
        {
            ResourceType = ResourceType.Image
        };

        var result = await _cloudinary.DestroyAsync(deletionParams);

        if (!string.Equals(result.Result, "ok", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(result.Result, "not found", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException($"Falha ao remover imagem do Cloudinary: {result.Result}");
        }
    }

    private static string ResolveUrl(ImageUploadResult result)
    {
        return result.SecureUrl?.ToString() ?? result.Url?.ToString() ?? string.Empty;
    }

    private static string ResolveContentType(string? requestedContentType, string? format)
    {
        if (!string.IsNullOrWhiteSpace(requestedContentType))
            return requestedContentType;

        if (!string.IsNullOrWhiteSpace(format))
            return $"image/{format}";

        return "image/jpeg";
    }
}

