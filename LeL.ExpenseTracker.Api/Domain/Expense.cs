using LeL.ExpenseTracker.Api.Domain.Base;

namespace LeL.ExpenseTracker.Api.Domain;

public class Expense : Entity<Guid>
{
    public Expense(string name, DateTime date, string description, decimal amount, CurrencyType currency, int expenseCategoryId)
    {
        Name = name;
        Date = date;
        Description = description;
        Amount = amount;
        Currency = currency;
        ExpenseCategoryId = expenseCategoryId;
    }

    public string Name { get; set; } = string.Empty;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    public string Description { get; set; } = string.Empty;

    public decimal Amount { get; set; } = 0.0m;

    public CurrencyType Currency { get; set; }

    public int ExpenseCategoryId { get; set; }

    public ExpenseCategory ExpenseCategory { get; set; } = null!;
}
