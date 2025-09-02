Feature: GetExpense

Background:
	Given a valid Expense
	And I saved the Expense

Scenario: Get an Expense
	Given I select an existing Expense
	When I get the Expense
	Then the Expense should be returned successfully

@validation
Scenario: Get an Expense with unexisting Id
	Given I select unexisting Expense
	When I get the Expense
	Then the Expense should not be found

@validation
Scenario: Get an Expense with empty Id
	Given I select an Expense with empty Id
	When I get the Expense
	Then the Expense should not be retrieved