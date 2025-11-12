using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class GaleriaValidation : AbstractValidator<Galeria>
{
    public GaleriaValidation()
    {
        RuleFor(g => g.IdEvento)
            .GreaterThan(0).WithMessage("O evento é obrigatório.");

        RuleFor(g => g.IdFoto)
            .GreaterThan(0).WithMessage("A foto é obrigatória.");

        RuleFor(g => g.Ordem)
            .GreaterThanOrEqualTo(0).WithMessage("A ordem deve ser maior ou igual a 0.");

        RuleFor(g => g.Visibilidade)
            .NotEmpty().WithMessage("A visibilidade é obrigatória.")
            .MaximumLength(50).WithMessage("A visibilidade deve ter até 50 caracteres.");

        RuleFor(g => g.Legenda)
            .MaximumLength(500).WithMessage("A legenda deve ter até 500 caracteres.");

        RuleFor(g => g.Data)
            .NotEmpty().WithMessage("A data é obrigatória.");
    }
}
