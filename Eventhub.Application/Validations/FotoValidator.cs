using FluentValidation;
using Eventhub.Application.DTOs;

namespace Eventhub.Application.Validations;

public class UploadFotoValidator : AbstractValidator<UploadFotoDto>
{
    public UploadFotoValidator()
    {
        RuleFor(f => f.NomeArquivo)
            .NotEmpty().WithMessage("Nome do arquivo é obrigatório.")
            .MaximumLength(255).WithMessage("Nome do arquivo muito longo.")
            .Must(nome => nome.EndsWith(".jpg") || nome.EndsWith(".jpeg") || nome.EndsWith(".png"))
            .WithMessage("Apenas arquivos .jpg, .jpeg ou .png são permitidos.");

        RuleFor(f => f.Base64)
            .NotEmpty().WithMessage("Arquivo é obrigatório.")
            .Must(FotoBase64Helper.IsBase64String).WithMessage("Arquivo não está em Base64 válido.");
    }
}

public class UpdateFotoValidator : AbstractValidator<UpdateFotoDto>
{
    public UpdateFotoValidator()
    {
        RuleFor(f => f.Id)
            .GreaterThan(0).WithMessage("Id inválido.");

        RuleFor(f => f.NomeArquivo)
            .NotEmpty().WithMessage("Nome do arquivo é obrigatório.")
            .MaximumLength(255).WithMessage("Nome do arquivo muito longo.")
            .Must(nome => nome.EndsWith(".jpg") || nome.EndsWith(".jpeg") || nome.EndsWith(".png"))
            .WithMessage("Apenas arquivos .jpg, .jpeg ou .png são permitidos.");

        RuleFor(f => f.Base64)
            .NotEmpty().WithMessage("Arquivo é obrigatório.")
            .Must(FotoBase64Helper.IsBase64String).WithMessage("Arquivo não está em Base64 válido.");
    }
}

internal static class FotoBase64Helper
{
    public static bool IsBase64String(string? base64)
    {
        if (string.IsNullOrWhiteSpace(base64))
            return false;

        var sanitized = Sanitize(base64);
        Span<byte> buffer = new Span<byte>(new byte[sanitized.Length]);
        return Convert.TryFromBase64String(sanitized, buffer, out _);
    }

    private static string Sanitize(string value)
    {
        var trimmed = value.Trim();
        const string dataPrefix = "data:";

        if (trimmed.StartsWith(dataPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var commaIndex = trimmed.IndexOf(',');
            if (commaIndex >= 0 && commaIndex < trimmed.Length - 1)
            {
                return trimmed[(commaIndex + 1)..];
            }
        }

        return trimmed;
    }
}
