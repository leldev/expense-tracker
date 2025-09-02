using LeL.ExpenseTracker.Api.Domain;

namespace LeL.ExpenseTracker.Specs.Support;

[Binding]
public class Hooks
{
    public static GlobalContext GlobalContext { get; private set; } = null!;

    [BeforeTestRun]
    public static void InitializeGlobalContext()
    {
        GlobalContext ??= new GlobalContext();
    }

    [BeforeTestRun]
    public static async Task InitializeDataAsync()
    {
        using var dbContext = GlobalContext.CreateDbContext();

        var categories = Enum.GetValues<ExpenseCategoryType>().Select(ct => new ExpenseCategory((int)ct, ct.ToString(), $"Description for {ct}"));

        await dbContext.ExpenseCategories.AddRangeAsync(categories);
        await dbContext.SaveChangesAsync();
    }

    [AfterScenario]
    public static async Task CleanExpensesAsync()
    {
        using var dbContext = GlobalContext.CreateDbContext();

        dbContext.Expenses.RemoveRange(dbContext.Expenses);

        await dbContext.SaveChangesAsync();
    }
}
