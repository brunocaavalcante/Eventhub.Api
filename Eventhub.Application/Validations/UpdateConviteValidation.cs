using FluentValidation;
using Eventhub.Application.DTOs;

namespace Eventhub.Application.Validations;

public class UpdateConviteValidation : AbstractValidator<UpdateConviteDto>
{
    public UpdateConviteValidation()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("O nome é obrigatório.")
            .MaximumLength(100).WithMessage("O nome deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Nome2)
            .MaximumLength(100).WithMessage("O nome2 deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Mensagem)
            .NotEmpty().WithMessage("A mensagem é obrigatória.")
            .MaximumLength(500).WithMessage("A mensagem deve ter no máximo 500 caracteres.");

        RuleFor(x => x.TemaConvite)
            .NotEmpty().WithMessage("O tema do convite é obrigatório.")
            .MaximumLength(100).WithMessage("O tema do convite deve ter no máximo 100 caracteres.");

        RuleFor(x => x.DataInicio)
            .LessThanOrEqualTo(x => x.DataFim).WithMessage("A data de início deve ser menor ou igual à data de fim.")
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue);

        RuleFor(x => x.DataFim)
            .GreaterThanOrEqualTo(x => x.DataInicio).WithMessage("A data de fim deve ser maior ou igual à data de início.")
            .When(x => x.DataInicio.HasValue && x.DataFim.HasValue);
    }
}
