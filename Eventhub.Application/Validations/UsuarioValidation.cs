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

        RuleFor(u => u.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("O e-mail deve ter até 200 caracteres.");

        RuleFor(u => u.Telefone)
            .MaximumLength(20).WithMessage("O telefone deve ter até 20 caracteres.");

        RuleFor(u => u.Status)
            .NotEmpty().WithMessage("O status é obrigatório.")
            .MaximumLength(50).WithMessage("O status deve ter até 50 caracteres.");
    }
}
