using Eventhub.Application.DTOs;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class CancelarReservaPresenteValidation : AbstractValidator<CancelarReservaPresenteDto>
{
    public CancelarReservaPresenteValidation()
    {
        RuleFor(c => c.IdParticipante)
            .GreaterThan(0).WithMessage("O participante é obrigatório.");

        RuleFor(c => c.Justificativa)
            .NotEmpty().WithMessage("A justificativa do cancelamento é obrigatória.")
            .MinimumLength(10).WithMessage("A justificativa deve ter no mínimo 10 caracteres.")
            .MaximumLength(500).WithMessage("A justificativa deve ter no máximo 500 caracteres.");
    }
}
