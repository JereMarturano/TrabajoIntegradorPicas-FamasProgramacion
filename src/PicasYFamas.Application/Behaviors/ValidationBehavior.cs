using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using PicasYFamas.Domain.Exceptions; // Using DomainException as a base or creating a ValidationException

namespace PicasYFamas.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
        var failures = validationResults.SelectMany(r => r.Errors).Where(f => f != null).ToList();

        if (failures.Count != 0)
        {
            var errors = string.Join(", ", failures.Select(f => f.ErrorMessage));
            throw new DomainException($"Validation failed: {errors}"); // Or a specific ValidationException
        }

        return await next();
    }
}
