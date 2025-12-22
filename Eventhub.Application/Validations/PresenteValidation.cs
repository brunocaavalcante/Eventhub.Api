using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class PresenteValidation : AbstractValidator<Presente>
{
    public PresenteValidation()
    {
        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("O nome do presente é obrigatório.")
            .MaximumLength(100).WithMessage("O nome do presente deve ter até 100 caracteres.");

        RuleFor(p => p.Descricao)
            .NotEmpty().WithMessage("A descrição do presente é obrigatória.")
            .MaximumLength(500).WithMessage("A descrição do presente deve ter até 500 caracteres.");

        RuleFor(p => p.Valor)
            .GreaterThan(0).WithMessage("O valor do presente deve ser maior que 0.");
    }
}