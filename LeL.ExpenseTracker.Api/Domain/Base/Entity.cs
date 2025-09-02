namespace LeL.ExpenseTracker.Api.Domain.Base;

public abstract class Entity<TId>
    where TId : notnull
{
    public TId Id { get; protected set; } = default!;
}