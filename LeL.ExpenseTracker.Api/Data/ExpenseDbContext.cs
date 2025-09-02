using LeL.ExpenseTracker.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace LeL.ExpenseTracker.Api.Data;

public class ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : DbContext(options)
{
    public const string ConnectionStringName = "Expense";

    public DbSet<Expense> Expenses => Set<Expense>();

    public DbSet<ExpenseCategory> ExpenseCategories => Set<ExpenseCategory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ExpenseDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
