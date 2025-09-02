using Bogus;
using LeL.ExpenseTracker.Api.Domain;
using LeL.ExpenseTracker.Api.Features.Expenses;
using LeL.ExpenseTracker.Api.Features.Expenses.CreateExpense;
using LeL.ExpenseTracker.Specs.Support;
using LeL.ExpenseTracker.Specs.Support.Extensions;
using LeL.ExpenseTracker.Specs.Support.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace LeL.ExpenseTracker.Specs.StepDefinitions.Expenses.CreateExpense
{
    [Binding]
    public class CreateExpenseStepDefinitions(ScenarioContext scenarioContext)
    {
        private readonly ScenarioContext scenarioContext = scenarioContext;
        private readonly ExpenseFunction expenseFunction = Hooks.GlobalContext.CreateExpenseFunction();

        [Given("a valid Expense")]
        public void GivenAValidExpense()
        {
            var createExpenseCommandBodyFaker = new Faker<CreateExpenseCommandBody>()
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.Date, f => f.Date.Recent())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Amount, f => f.Finance.Amount())
                .RuleFor(x => x.Currency, f => f.PickRandom<CurrencyType>())
                .RuleFor(x => x.ExpenseCategoryId, f => (int)f.PickRandom<ExpenseCategoryType>());

            scenarioContext.Set(createExpenseCommandBodyFaker);
        }

        [Given("I left (.*) empty")]
        public void GivenILeftNameEmpty(string propName)
        {
            var faker = scenarioContext.Get<Faker<CreateExpenseCommandBody>>();

            var property = typeof(CreateExpenseCommandBody).GetProperty(propName);

            if (property is null)
            {
                throw new ArgumentException($"Property '{propName}' does not exist in CreateExpenseCommandBody.", nameof(propName));
            }

            var defaultValue = property.PropertyType switch
            {
                Type t when t == typeof(string) => (Func<Faker, object>)(f => string.Empty),
                Type t when t == typeof(int) => f => 0,
                Type t when t == typeof(decimal) => f => 0.0m,
                Type t when t == typeof(DateTime) => f => DateTime.MinValue,
                Type t when t.IsEnum => f => 0,
                _ => throw new NotSupportedException($"Property type '{property.PropertyType}' is not supported.")
            };

            faker.RuleFor(propName, defaultValue);
        }

        [Given("the Category does not exist")]
        public void GivenTheCategoryDoesNotExist()
        {
            var faker = scenarioContext.Get<Faker<CreateExpenseCommandBody>>();

            faker.RuleFor(x => x.ExpenseCategoryId, int.MaxValue);
        }

        [Given(@"a valid \$([0-9.]+) Expense for the (.+) category.")]
        [Given(@"\$([0-9.]+) Expense for the (.+) category.")]
        public void GivenAValidAmountExpenseForTheCategory(float amount, string category)
        {
            if (!Enum.TryParse<ExpenseCategoryType>(category, ignoreCase: true, out var categoryEnum))
            {
                throw new ArgumentException($"Category '{category}' is not a valid ExpenseCategoryType.");
            }

            var createExpenseCommandBodyFaker = new Faker<CreateExpenseCommandBody>()
                .RuleFor(x => x.Name, f => f.Lorem.Word())
                .RuleFor(x => x.Date, f => f.Date.Recent())
                .RuleFor(x => x.Description, f => f.Lorem.Sentence())
                .RuleFor(x => x.Amount, _ => (decimal)amount)
                .RuleFor(x => x.Currency, f => f.PickRandom<CurrencyType>())
                .RuleFor(x => x.ExpenseCategoryId, _ => (int)categoryEnum);

            scenarioContext.Set(createExpenseCommandBodyFaker);
        }

        [Given("I saved the Expense")]
        [When("I save the Expense")]
        public async Task WhenISaveTheExpenseAsync()
        {
            var faker = scenarioContext.Get<Faker<CreateExpenseCommandBody>>();
            var body = faker.Generate();
            var httpRequest = HttpRequestHelper.CreateHttpRequest(body);

            var result = await expenseFunction.CreateExpenseAsync(httpRequest);

            var objectResult = result as ObjectResult;

            if (objectResult is not null && objectResult.Value is CreateExpenseResponse createExpenseResponse)
            {
                scenarioContext.Set(createExpenseResponse);
            }

            scenarioContext.Set(result);
        }

        [Then("the Expense should be saved successfully")]
        public void ThenTheExpenseShouldBeSavedSuccessfully()
        {
            var result = scenarioContext.Get<IActionResult>();

            result.AssertWithStatusCode(HttpStatusCode.Created);
        }

        [Then("the Expense should not be created")]
        public void ThenTheExpenseShouldNotBeCreated()
        {
            var result = scenarioContext.Get<IActionResult>();

            result.AssertWithStatusCode(HttpStatusCode.BadRequest);
        }

        [Then("the Expense should not be created with a precondition error")]
        public void ThenTheExpenseShouldNotBeCreatedWithAPreconditionError()
        {
            var result = scenarioContext.Get<IActionResult>();

            result.AssertWithStatusCode(HttpStatusCode.PreconditionFailed);
        }
    }
}
