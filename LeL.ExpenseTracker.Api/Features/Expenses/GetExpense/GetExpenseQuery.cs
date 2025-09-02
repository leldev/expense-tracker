using AutoMapper;
using AutoMapper.QueryableExtensions;
using LeL.ExpenseTracker.Api.Data;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeL.ExpenseTracker.Api.Features.Expenses.GetExpense;

public sealed record GetExpenseQuery : IRequest<IActionResult>
{
    [FromRoute]
    public Guid ExpenseId { get; init; }
}

public sealed class GetExpensaByIdQueryHandler(ExpenseDbContext dbContext, IMapper mapper) : IRequestHandler<GetExpenseQuery, IActionResult>
{
    private readonly ExpenseDbContext dbContext = dbContext;
    private readonly IMapper mapper = mapper;

    public async Task<IActionResult> Handle(GetExpenseQuery request, CancellationToken cancellationToken)
    {
        var expenseResponse = await dbContext.Expenses.AsNoTracking()
            .Where(x => x.Id == request.ExpenseId)
            .ProjectTo<GetExpenseResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken);

        if (expenseResponse is null)
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status404NotFound,
            };
        }

        return new OkObjectResult(expenseResponse);
    }
}