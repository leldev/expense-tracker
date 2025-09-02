using FluentValidation;

namespace LeL.ExpenseTracker.Api.Features.Expenses.CreateExpense;

public class CreateExpenseValidator : AbstractValidator<CreateExpenseCommand>
{
    public CreateExpenseValidator()
    {
        RuleFor(x => x.Body).NotNull();

        When(x => x.Body is not null, () =>
        {
            RuleFor(x => x.Body.Name).NotEmpty();
            RuleFor(x => x.Body.Description).NotEmpty();
            RuleFor(x => x.Body.Date).NotEmpty();
            RuleFor(x => x.Body.Amount).GreaterThan(0);
            RuleFor(x => x.Body.Currency).IsInEnum();
            RuleFor(x => x.Body.ExpenseCategoryId).GreaterThan(0);
        });
    }
}