using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class PixEventoValidation : AbstractValidator<PixEvento>
{
    public PixEventoValidation()
    {
        RuleFor(p => p.IdEvento)
            .GreaterThan(0)
            .WithMessage("O evento é obrigatório.");

        RuleFor(p => p.NomeBeneficiario)
            .NotEmpty()
            .WithMessage("O nome do beneficiário é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do beneficiário deve ter no máximo 200 caracteres.");

        RuleFor(p => p.QRCodePix)
            .NotEmpty()
            .WithMessage("O código QR Code PIX é obrigatório.")
            .MaximumLength(1000)
            .WithMessage("O código QR Code PIX deve ter no máximo 1000 caracteres.")
            .Must(ValidarFormatoQRCodePix)
            .WithMessage("O QR Code PIX deve começar com '00020' (formato válido de payload PIX).");
    }

    private bool ValidarFormatoQRCodePix(string qrCodePix)
    {
        if (string.IsNullOrWhiteSpace(qrCodePix))
            return false;

        return qrCodePix.Trim().StartsWith("00020");
    }
}