using FluentValidation;

namespace LeL.ExpenseTracker.Api.Features.Expenses.GetExpense;

public class GetExpenseValidator : AbstractValidator<GetExpenseQuery>
{
    public GetExpenseValidator()
    {
        RuleFor(x => x.ExpenseId).NotEmpty();
    }
}