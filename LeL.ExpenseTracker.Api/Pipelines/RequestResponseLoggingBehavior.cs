using LeL.ExpenseTracker.Api.Extensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LeL.ExpenseTracker.Api.Pipelines;

public class RequestResponseLoggingBehavior<TRequest, TResponse>(ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<RequestResponseLoggingBehavior<TRequest, TResponse>> logger = logger;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("Request Name: {Name}", typeof(TRequest).Name);

        logger.LogInformation("Request Details: {Request}", request.Serialize());

        var response = await next(cancellationToken);

        if (response is IActionResult actionResult)
        {
            object? responseValue = actionResult switch
            {
                ObjectResult obj => obj.Value,
                JsonResult json => json.Value,
                ContentResult content => content.Content,
                StatusCodeResult status => $"Status code: {status.StatusCode}",
                _ => "No content"
            };

            logger.LogInformation("Response: {ResponseValue}", responseValue.Serialize());
        }

        return response;
    }
}
