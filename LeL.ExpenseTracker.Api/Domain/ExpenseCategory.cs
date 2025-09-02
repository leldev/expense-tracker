using LeL.ExpenseTracker.Api.Domain.Base;

namespace LeL.ExpenseTracker.Api.Domain;

public class ExpenseCategory : Entity<int>
{
    public ExpenseCategory(int Id, string name, string description)
    {
        this.Id = Id;
        this.Name = name;
        this.Description = description;
    }

    public string Name { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public IList<Expense> Expenses { get; set; } = [];
}