using Eventhub.Application.DTOs.Evento;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class CancelarEventoValidation : AbstractValidator<CancelarEventoDto>
{
    public CancelarEventoValidation()
    {
        RuleFor(x => x.Justificativa)
            .NotEmpty()
            .WithMessage("A justificativa do cancelamento é obrigatória.")
            .MinimumLength(10)
            .WithMessage("A justificativa deve ter no mínimo 10 caracteres.")
            .MaximumLength(500)
            .WithMessage("A justificativa deve ter no máximo 500 caracteres.");
    }
}
