Feature: CreateExpense

Scenario: Create an Expense
	Given a valid Expense
	When I save the Expense
	Then the Expense should be saved successfully

Scenario Outline: Create an Expense for each category type
	Given a valid $<Amount> Expense for the <Category> category.
	When I save the Expense
	Then the Expense should be saved successfully
Scenarios: 
	| Amount | Category       |
	| 9.99   | FoodAndDining  |
	| 2.9    | Transportation |
	| 1200   | Housing        |
	| 32     | Utilities      |
	| 100    | Healthcare     |
	| 14.5   | Entertainment  |
	| 25     | Shopping       |
	| 3200   | Education      |
	| 77.8   | Travel         |
	| 182    | Business       |

@validation
Scenario: Create an Expense with invalid Category
	Given a valid Expense
	But the Category does not exist
	When I save the Expense
	Then the Expense should not be created with a precondition error

@validation
Scenario Outline: Create an Expense with required fields cannot be empty
	Given a valid Expense
	But I left <Field> empty
	When I save the Expense
	Then the Expense should not be created
Scenarios:
	| Field             |
	| Name              |
	| Date              |
	| Description       |
	| Amount            |
	| Currency          |
	| ExpenseCategoryId |
