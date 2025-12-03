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
            .Must(IsBase64String).WithMessage("Arquivo não está em Base64 válido.");
    }

    private bool IsBase64String(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return false;
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
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
            .Must(IsBase64String).WithMessage("Arquivo não está em Base64 válido.");
    }

    private bool IsBase64String(string base64)
    {
        if (string.IsNullOrWhiteSpace(base64)) return false;
        Span<byte> buffer = new Span<byte>(new byte[base64.Length]);
        return Convert.TryFromBase64String(base64, buffer, out _);
    }
}
