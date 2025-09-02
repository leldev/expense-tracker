using LeL.ExpenseTracker.Api.Features.Expenses;
using LeL.ExpenseTracker.Api.Features.Expenses.CreateExpense;
using LeL.ExpenseTracker.Api.Features.Expenses.GetExpense;
using LeL.ExpenseTracker.Specs.Support;
using LeL.ExpenseTracker.Specs.Support.Extensions;
using LeL.ExpenseTracker.Specs.Support.Helpers;
using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Net;

namespace LeL.ExpenseTracker.Specs.StepDefinitions.Expenses.GetExpense;

[Binding]
public class GetExpenseStepDefinitions(ScenarioContext scenarioContext)
{
    private readonly ScenarioContext scenarioContext = scenarioContext;
    private readonly ExpenseFunction expenseFunction = Hooks.GlobalContext.CreateExpenseFunction();

    [Given("I select an existing Expense")]
    public void GivenISelectAnExistingExpense()
    {
        var createExpenseResponse = scenarioContext.Get<CreateExpenseResponse>();

        scenarioContext.Set(createExpenseResponse.Id, "ExpenseId");
    }

    [Given("I select unexisting Expense")]
    public void GivenISelectUnexistingExpense()
    {
        scenarioContext.Set(Guid.NewGuid(), "ExpenseId");
    }

    [Given("I select an Expense with empty Id")]
    public void GivenISelectAnExpenseWithEmptyId()
    {
        scenarioContext.Set(Guid.Empty, "ExpenseId");
    }

    [When("I get the Expense")]
    public async Task WhenIGetTheExpenseAsync()
    {
        var expenseId = scenarioContext.Get<Guid>("ExpenseId");

        var route = new Dictionary<string, string>
        {
            { "expenseId", expenseId.ToString() }
        };

        var httpRequest = HttpRequestHelper.CreateHttpRequest(route: route);

        var result = await expenseFunction.GetExpenseAsync(httpRequest);

        scenarioContext.Set(result);
    }

    [Then("the Expense should be returned successfully")]
    public void ThenTheExpenseShouldBeReturnedSuccessfully()
    {
        var result = scenarioContext.Get<IActionResult>();

        result.AssertWithStatusCode(HttpStatusCode.OK);

        var objectResult = result as ObjectResult;

        var getExpensesResponse = objectResult!.Value as GetExpenseResponse;

        getExpensesResponse.ShouldNotBeNull();

        var expenseId = scenarioContext.Get<Guid>("ExpenseId");

        getExpensesResponse.Id.ShouldBe(expenseId);
        getExpensesResponse.Amount.ShouldBeGreaterThan(0);
        getExpensesResponse.CurrencyName.ShouldNotBeNullOrEmpty();
        getExpensesResponse.Date.ShouldNotBe(DateTime.MinValue);
        getExpensesResponse.Description.ShouldNotBeNullOrEmpty();
        getExpensesResponse.ExpenseCategoryId.ShouldBeGreaterThan(0);
        getExpensesResponse.ExpenseCategoryName.ShouldNotBeNullOrEmpty();
        getExpensesResponse.Name.ShouldNotBeNullOrEmpty();
    }

    [Then("the Expense should not be found")]
    public void ThenTheExpenseShouldNotBeFound()
    {
        var result = scenarioContext.Get<IActionResult>();

        result.AssertWithStatusCode(HttpStatusCode.NotFound);
    }

    [Then("the Expense should not be retrieved")]
    public void ThenTheExpenseShouldNotBeRetrieved()
    {
        var result = scenarioContext.Get<IActionResult>();

        result.AssertWithStatusCode(HttpStatusCode.BadRequest);
    }
}
