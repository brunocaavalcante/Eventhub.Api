using Eventhub.Domain.Exceptions;
using FluentValidation;

namespace Eventhub.Application.Services;

public abstract class BaseService
{
    protected bool ExecutarValidacao<TV, TE>(TV validacao, TE entidade)
                where TV : AbstractValidator<TE>
                where TE : class
    {
        var validator = validacao.Validate(entidade);
        if (validator.IsValid) return true;
        throw new ExceptionValidation(string.Join("; ", validator.Errors.Select(e => e.ErrorMessage)));
    }
}
