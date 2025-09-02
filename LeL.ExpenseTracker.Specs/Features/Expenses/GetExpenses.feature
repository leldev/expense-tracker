Feature: GetExpenses

Scenario: Get Expenses
	Given a valid Expense
	And I saved the Expense
	When I get the Expenses
	Then the Expenses should be returned successfully

@userstory
Scenario: I went on vacation and now want to record every expense.
	Given $9.99 Expense for the FoodAndDining category.
	And I saved the Expense
	And $2.9 Expense for the Transportation category.
	And I saved the Expense
	And $25 Expense for the Shopping category.
	And I saved the Expense
	And $77.8 Expense for the Travel category.
	And I saved the Expense
	When I get the Expenses
	Then I should get a total of $115.69 for my vacation expenses