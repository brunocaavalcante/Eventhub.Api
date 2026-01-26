using Eventhub.Application.DTOs;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class CancelarContribuicaoPresenteValidation : AbstractValidator<CancelarContribuicaoPresenteDto>
{
    public CancelarContribuicaoPresenteValidation()
    {
        RuleFor(x => x.IdContribuicao)
            .GreaterThan(0).WithMessage("ID da contribuição inválido.");

        RuleFor(x => x.Justificativa)
            .NotEmpty().WithMessage("A justificativa do cancelamento é obrigatória.")
            .MaximumLength(500).WithMessage("A justificativa deve ter no máximo 500 caracteres.");
    }
}
