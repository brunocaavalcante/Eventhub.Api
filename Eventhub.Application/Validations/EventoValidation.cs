using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class EventoValidation : AbstractValidator<Evento>
{
    public EventoValidation()
    {
        RuleFor(e => e.Descricao)
            .NotEmpty().WithMessage("A descrição do evento é obrigatória.")
            .MaximumLength(200).WithMessage("A descrição do evento deve ter até 200 caracteres.");

        RuleFor(e => e.DataInicio)
            .NotEmpty().WithMessage("A data de início é obrigatória.");

        RuleFor(e => e.DataFim)
            .NotEmpty().WithMessage("A data de fim é obrigatória.")
            .GreaterThanOrEqualTo(e => e.DataInicio).WithMessage("A data de fim deve ser maior ou igual à data de início.");
    }
}
