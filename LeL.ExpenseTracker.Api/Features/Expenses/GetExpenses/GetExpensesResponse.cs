using AutoMapper;
using LeL.ExpenseTracker.Api.Domain;
using LeL.ExpenseTracker.Api.Mappings;

namespace LeL.ExpenseTracker.Api.Features.Expenses.GetExpenses;

public class GetExpensesResponse : IMappable<Expense>
{
    public Guid Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public DateTime Date { get; init; }

    public string Description { get; init; } = string.Empty;

    public decimal Amount { get; init; }

    public CurrencyType Currency { get; init; }

    public string CurrencyName { get; init; } = string.Empty;

    public int ExpenseCategoryId { get; init; }

    public string ExpenseCategoryName { get; init; } = string.Empty;

    public void CreateMap(Profile profile)
    {
        profile.CreateMap<Expense, GetExpensesResponse>()
            .ForMember(d => d.ExpenseCategoryName, opt => opt.MapFrom(s => s.ExpenseCategory.Name))
            .ForMember(d => d.CurrencyName, opt => opt.MapFrom(s => s.Currency.ToString()));
    }
}
