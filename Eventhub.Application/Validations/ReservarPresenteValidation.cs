using Eventhub.Application.DTOs;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class ReservarPresenteValidation : AbstractValidator<ReservarPresenteDto>
{
    public ReservarPresenteValidation()
    {
        RuleFor(r => r.IdParticipante)
            .GreaterThan(0).WithMessage("O participante é obrigatório.");
    }
}
