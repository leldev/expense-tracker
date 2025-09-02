using Microsoft.AspNetCore.Mvc;
using Shouldly;
using System.Net;

namespace LeL.ExpenseTracker.Specs.Support.Extensions;

public static class ActionResultExtension
{
    public static void AssertWithStatusCode(this IActionResult actionResult, HttpStatusCode statusCode)
    {
        actionResult.ShouldNotBeNull();

        var objectResult = actionResult as ObjectResult;

        objectResult!.StatusCode.ShouldBe((int)statusCode);
    }
}
