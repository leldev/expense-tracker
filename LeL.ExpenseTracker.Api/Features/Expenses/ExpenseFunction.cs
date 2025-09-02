using LeL.ExpenseTracker.Api.Extensions;
using LeL.ExpenseTracker.Api.Features.Expenses.CreateExpense;
using LeL.ExpenseTracker.Api.Features.Expenses.GetExpense;
using LeL.ExpenseTracker.Api.Features.Expenses.GetExpenses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.OpenApi.Models;
using System.Net;

namespace LeL.ExpenseTracker.Api.Features.Expenses;

public class ExpenseFunction(ISender mediator)
{
    private readonly ISender mediator = mediator;

    [Function(nameof(CreateExpenseAsync))]
    [OpenApiOperation(nameof(CreateExpenseAsync))]
    [OpenApiRequestBody("application/json", typeof(CreateExpenseCommandBody), Required = true)]
    [OpenApiResponseWithBody(HttpStatusCode.Created, "application/json", typeof(CreateExpenseResponse))]
    public async Task<IActionResult> CreateExpenseAsync([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "expenses")] HttpRequest req)
    {
        return await mediator.Send(await req.BindToCommandQueryAsync<CreateExpenseCommand>());
    }
    
    [Function(nameof(GetExpenseAsync))]
    [OpenApiOperation(nameof(GetExpenseAsync))]
    [OpenApiParameter("expenseId", In = ParameterLocation.Path, Type = typeof(Guid))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(GetExpenseResponse))]
    [OpenApiResponseWithoutBody(HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetExpenseAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses/{expenseId:guid}")] HttpRequest req)
    {
        return await mediator.Send(await req.BindToCommandQueryAsync<GetExpenseQuery>());
    }

    [Function(nameof(GetExpensesAsync))]
    [OpenApiOperation(nameof(GetExpensesAsync))]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<GetExpensesResponse>))]
    public async Task<IActionResult> GetExpensesAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "expenses")] HttpRequest req)
    {
        return await mediator.Send(await req.BindToCommandQueryAsync<GetExpensesQuery>());
    }
}
