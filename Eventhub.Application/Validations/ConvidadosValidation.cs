using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class ConvidadosValidation : AbstractValidator<Convidados>
{
    public ConvidadosValidation()
    {
        RuleFor(c => c.IdEvento)
            .GreaterThan(0).WithMessage("O evento é obrigatório.");

        RuleFor(c => c.IdFoto)
            .GreaterThan(0).WithMessage("A foto é obrigatória.");

        RuleFor(c => c.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(200).WithMessage("O nome deve ter até 200 caracteres.");

        RuleFor(c => c.Email)
            .NotEmpty().WithMessage("O e-mail é obrigatório.")
            .EmailAddress().WithMessage("E-mail inválido.")
            .MaximumLength(200).WithMessage("O e-mail deve ter até 200 caracteres.");

        RuleFor(c => c.Telefone)
            .NotEmpty().WithMessage("O telefone é obrigatório.")
            .MaximumLength(20).WithMessage("O telefone deve ter até 20 caracteres.");
    }
}
