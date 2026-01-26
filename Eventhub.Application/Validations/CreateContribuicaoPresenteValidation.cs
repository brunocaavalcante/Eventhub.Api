using Eventhub.Application.DTOs;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class CreateContribuicaoPresenteValidation : AbstractValidator<CreateContribuicaoPresenteDto>
{
    public CreateContribuicaoPresenteValidation()
    {
        RuleFor(x => x.IdPresente)
            .GreaterThan(0).WithMessage("ID do presente inválido.");

        RuleFor(x => x.IdParticipante)
            .GreaterThan(0).WithMessage("ID do participante inválido.");

        RuleFor(x => x.Valor)
            .GreaterThan(0).WithMessage("O valor da contribuição deve ser maior que 0.");

        RuleFor(x => x.FormaPagamento)
            .NotEmpty().WithMessage("Forma de pagamento é obrigatória.")
            .MaximumLength(100).WithMessage("Forma de pagamento deve ter no máximo 100 caracteres.");

        RuleFor(x => x.Comprovante)
            .NotNull().WithMessage("O comprovante é obrigatório.")
            .SetValidator(new UploadFotoValidator());
    }
}
