using AutoMapper;
using AutoMapper.QueryableExtensions;
using LeL.ExpenseTracker.Api.Data;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeL.ExpenseTracker.Api.Features.Expenses.GetExpenses;

public sealed record GetExpensesQuery : IRequest<IActionResult>
{
    [FromQuery]
    public int? ExpenseCategoryId { get; init; }
}

public sealed class GetExpensesQueryHandler(ExpenseDbContext dbContext, IMapper mapper) : IRequestHandler<GetExpensesQuery, IActionResult>
{
    private readonly ExpenseDbContext dbContext = dbContext;
    private readonly IMapper mapper = mapper;

    public async Task<IActionResult> Handle(GetExpensesQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Expenses.AsNoTracking();

        if (request.ExpenseCategoryId.HasValue)
        {
            query = query.Where(e => e.ExpenseCategoryId == request.ExpenseCategoryId.Value);
        }

        var expensesResponse = await query
            .ProjectTo<GetExpensesResponse>(mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        return new OkObjectResult(expensesResponse);
    }
}