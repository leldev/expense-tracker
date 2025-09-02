using LeL.ExpenseTracker.Api.Extensions;
using Microsoft.AspNetCore.Http;
using System.Text;

namespace LeL.ExpenseTracker.Specs.Support.Helpers;

public static class HttpRequestHelper
{
    public static HttpRequest CreateHttpRequest(object? body = null, Dictionary<string, string> route = null)
    {
        var context = new DefaultHttpContext();
        var request = context.Request;
        request.ContentType = "application/json";

        if (route is not null)
        {
            foreach (var (key, value) in route)
            {
                request.RouteValues[key] = value;
            }
        }

        if (body is not null)
        {
            request.Body = new MemoryStream(Encoding.UTF8.GetBytes(body.Serialize()));
        }

        return request;
    }
}
