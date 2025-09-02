using FluentValidation;
using LeL.ExpenseTracker.Api.Data;
using LeL.ExpenseTracker.Api.Features.Expenses;
using LeL.ExpenseTracker.Api.Pipelines;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LeL.ExpenseTracker.Specs.Support;

public class GlobalContext
{
    public IServiceProvider ServiceProvider { get; }

    // Changed to create new DbContext instances per call instead of singleton
    public ExpenseDbContext CreateDbContext() => ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ExpenseDbContext>();

    public ExpenseFunction CreateExpenseFunction() => new(ServiceProvider.CreateScope().ServiceProvider.GetRequiredService<ISender>());

    public GlobalContext()
    {
        ServiceProvider = new ServiceCollection()
            .AddDbContext<ExpenseDbContext>(o => o.UseInMemoryDatabase(nameof(ExpenseDbContext)))
            .AddLogging()
            .AddAutoMapper(typeof(Program).Assembly)
            .AddMediatR(c => c.RegisterServicesFromAssembly(typeof(Program).Assembly))
            .AddValidatorsFromAssembly(typeof(Program).Assembly)
            .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
            .BuildServiceProvider();
    }
}
