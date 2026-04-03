using Eventhub.Application.DTOs.Evento;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class UpdateEventoValidation : AbstractValidator<UpdateEventoDto>
{
    public UpdateEventoValidation()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("O ID do evento é obrigatório.");
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O nome do evento é obrigatório.");
        RuleFor(x => x.DataInicio).NotEmpty().WithMessage("A data de início do evento é obrigatória.");
        RuleFor(x => x.DataFim).NotEmpty().WithMessage("A data de término do evento é obrigatória.");

        RuleFor(x => x.DataFim)
            .GreaterThan(x => x.DataInicio)
            .WithMessage("A data de término deve ser posterior à data de início.");

        RuleFor(x => x.Descricao).MaximumLength(500).WithMessage("A descrição do evento deve ter no máximo 500 caracteres.");

        RuleFor(x => x.Endereco).NotEmpty().WithMessage("O endereço do evento é obrigatório.");
        RuleFor(x => x.MaxConvidado).GreaterThan(0).WithMessage("A capacidade do evento deve ser maior que zero.");
    }
}
