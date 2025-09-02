# CQRS with Azure Function App + Reqnroll (BDD)

This project follows the same [CQRS pattern](https://github.com/leldev/CQRS) as the previous implementation, but instead of using a traditional Web API, it leverages **Azure Function App** with **HTTP Triggers** and uses **Reqnroll** for BDD testing.

The objective is to demonstrate how to apply **CQRS, MediatR, FluentValidation, AutoMapper, and BDD** in a **serverless environment** while keeping a clean and testable architecture.

---

## 🚀 Key Differences from the API Project

Compared to the previous Web API project, there are a few adjustments required when working with Azure Functions:

- **Request Binding**

  - In Function Apps, the trigger argument is always an `HttpRequest`.
  - We cannot directly map the request body into our command/query objects.
  - To solve this, an **extension method** was created to bind the request using **Body**, **Route**, or **Query** parameters.
  - Example:

    ```csharp
    return await mediator.Send(
        await req.BindToCommandQueryAsync<CreateExpenseCommand>()
    );
    ```

- **FluentValidation**

  - A **Pipeline Behavior** was implemented to validate commands/requests automatically before they reach their handler.

- **Swagger**

  - Swagger/OpenAPI works, but each function method must be decorated with attributes individually.
  - There isn’t a global extension to map function arguments automatically, since requests are passed as `HttpRequest`.

- **AutoMapper**

  - Configured and available for DTO ↔ entity mapping. ✅

- **BDD with Reqnroll**

  - Instead of SpecFlow (now deprecated), the project uses **Reqnroll** for writing **feature specs** and step definitions.
  - Syntax and usage are nearly identical to SpecFlow.

- **Testing**
  - **xUnit** – test framework
  - **Moq** – mocking dependencies
  - **Bogus** – generating fake data
  - **Shouldly** – expressive assertions ✅

---

## 📦 Tech Stack

- **Azure Function App (Isolated .NET 8)**
- **MediatR** – command/query handling
- **Entity Framework Core**
- **FluentValidation** – request validation
- **AutoMapper**
- **Swagger/OpenAPI**
- **Reqnroll** – BDD
- **xUnit / Moq / Bogus / Shouldly** – testing

---
