using Eventhub.Domain.Entities;
using FluentValidation;

namespace Eventhub.Application.Validations;

public class ParticipanteValidation : AbstractValidator<Participante>
{
    public ParticipanteValidation()
    {
        RuleFor(p => p.IdEvento)
            .GreaterThan(0).WithMessage("O evento é obrigatório.");

        RuleFor(p => p.IdUsuario)
            .GreaterThan(0).WithMessage("O usuário é obrigatório.");

        RuleFor(p => p.IdPerfil)
            .GreaterThan(0).WithMessage("O perfil é obrigatório.");
    }
}
