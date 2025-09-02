using LeL.ExpenseTracker.Api.Data;
using LeL.ExpenseTracker.Api.Domain;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeL.ExpenseTracker.Api.Features.Expenses.CreateExpense;

public sealed record CreateExpenseCommand : IRequest<IActionResult>
{
    [FromBody]
    public CreateExpenseCommandBody Body { get; init; } = null!;
}

public sealed record CreateExpenseCommandBody
{
    public string Name { get; init; } = string.Empty;

    public DateTime Date { get; init; } = DateTime.UtcNow;

    public string Description { get; init; } = string.Empty;

    public decimal Amount { get; init; } = 0.0m;

    public CurrencyType Currency { get; init; } = CurrencyType.USD;

    public int ExpenseCategoryId { get; init; } = 0;
}

public sealed class GetExpensaByIdQueryHandler(ExpenseDbContext dbContext) : IRequestHandler<CreateExpenseCommand, IActionResult>
{
    private readonly ExpenseDbContext dbContext = dbContext;

    public async Task<IActionResult> Handle(CreateExpenseCommand request, CancellationToken cancellationToken)
    {
        var existingExpenseCategory = await dbContext.ExpenseCategories.AsNoTracking()
            .Where(ec => ec.Id == request.Body.ExpenseCategoryId)
            .AnyAsync(cancellationToken);

        if (!existingExpenseCategory)
        {
            return new ObjectResult("Expense category not found")
            {
                StatusCode = StatusCodes.Status412PreconditionFailed,
            };
        }

        var expense = new Expense(request.Body.Name, request.Body.Date, request.Body.Description, request.Body.Amount, request.Body.Currency, request.Body.ExpenseCategoryId);
        
        await dbContext.Expenses.AddAsync(expense, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new ObjectResult(new CreateExpenseResponse(expense.Id))
        {
            StatusCode = StatusCodes.Status201Created
        };
    }
}