using LeL.ExpenseTracker.Api.Data;
using LeL.ExpenseTracker.Api.Features.Expenses;
using LeL.ExpenseTracker.Api.Features.Expenses.GetExpenses;
using LeL.ExpenseTracker.Specs.Support;
using LeL.ExpenseTracker.Specs.Support.Extensions;
using LeL.ExpenseTracker.Specs.Support.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Net;

namespace LeL.ExpenseTracker.Specs.StepDefinitions.Expenses.GetExpenses;

[Binding]
public class GetExpensesStepDefinitions(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext scenarioContext = scenarioContext;
    private readonly ExpenseFunction expenseFunction = Hooks.GlobalContext.CreateExpenseFunction();
    private readonly ExpenseDbContext dbContext = Hooks.GlobalContext.CreateDbContext();

    [When("I get the Expenses")]
    public async Task WhenIGetTheExpensesAsync()
    {
        var httpRequest = HttpRequestHelper.CreateHttpRequest();

        var result = await expenseFunction.GetExpensesAsync(httpRequest);

        scenarioContext.Set(result);
    }

    [Then("the Expenses should be returned successfully")]
    public void ThenTheExpensesShouldBeReturnedSuccessfully()
    {
        var result = scenarioContext.Get<IActionResult>();

        result.AssertWithStatusCode(HttpStatusCode.OK);

        var objectResult = result as ObjectResult;

        var getExpensesResponse = objectResult!.Value as List<GetExpensesResponse>;

        getExpensesResponse.ShouldNotBeEmpty();
    }

    [Then(@"I should get a total of \$([0-9.]+) for my vacation expenses")]
    public async Task ThenIShouldGetATotalOfForMyVacationExpensesAsync(decimal total)
    {
        var totalAmount = await dbContext.Expenses.SumAsync(x => x.Amount);

        total.ShouldBe(totalAmount);
    }
}
