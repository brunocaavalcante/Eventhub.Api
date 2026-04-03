using Eventhub.Application.Helpers;
using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class UsuarioValidation : AbstractValidator<Usuario>
{
    public UsuarioValidation()
    {
        RuleFor(u => u.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter até 200 caracteres.");

        // Validação condicional de email: emails temporários (@eventhub.temp) apenas validam tamanho
        // Emails reais exigem formato válido e obrigatoriedade
        RuleFor(u => u.Email)
            .Must((usuario, email) => !string.IsNullOrWhiteSpace(email) || EmailHelper.EhEmailTemporario(email))
            .WithMessage("O e-mail é obrigatório.")
            .Must((usuario, email) => EmailHelper.EhEmailTemporario(email) || IsValidEmail(email))
            .WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("O e-mail deve ter até 200 caracteres.");

        RuleFor(u => u.Telefone)
            .MaximumLength(20).WithMessage("O telefone deve ter até 20 caracteres.");

        RuleFor(u => u.Status)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .MaximumLength(50).WithMessage("O status deve ter até 50 caracteres.");
    }

    /// <summary>
    /// Valida se o email tem formato válido (não-vazio, contém @, etc).
    /// </summary>
    private static bool IsValidEmail(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
