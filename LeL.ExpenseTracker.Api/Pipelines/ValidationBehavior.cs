using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LeL.ExpenseTracker.Api.Pipelines;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> validators = validators;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count != 0)
        {
            if (typeof(TResponse) == typeof(IActionResult))
            {
                var validationProblemDetails = new ValidationProblemDetails(
                    failures.GroupBy(e => e.PropertyName).ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()));

                return (TResponse)(object)new BadRequestObjectResult(validationProblemDetails);
            }

            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}