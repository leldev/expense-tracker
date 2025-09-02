using System.Reflection;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LeL.ExpenseTracker.Api.Extensions;

public static class HttpRequestBindingExtensions
{
    public static async Task<T> BindToCommandQueryAsync<T>(this HttpRequest req)
        where T : new()
    {
        var command = Activator.CreateInstance<T>();
        var properties = typeof(T).GetProperties();

        ProcessFromRoute(req, command, properties);
        ProcessFromQuery(req, command, properties);
        await ProcessBodyContent(req, command!, properties);

        return command;
    }

    private static void ProcessFromRoute<T>(HttpRequest req, T? command, PropertyInfo[] properties) where T : new()
    {
        var routeData = ExtractRouteData(req);

        foreach (var prop in properties.Where(p => p.GetCustomAttribute<FromRouteAttribute>() is not null))
        {
            var paramName = prop.Name.ToLowerInvariant();

            if (routeData.TryGetValue(paramName, out var routeValue) && routeValue is not null)
            {
                SetPropertyValue(prop, command!, routeValue);
            }
        }
    }

    private static void ProcessFromQuery<T>(HttpRequest req, T? command, PropertyInfo[] properties) where T : new()
    {
        foreach (var prop in properties.Where(p => p.GetCustomAttribute<FromQueryAttribute>() is not null))
        {
            var queryAttr = prop.GetCustomAttribute<FromQueryAttribute>();
            var paramName = string.IsNullOrEmpty(queryAttr?.Name) ? prop.Name : queryAttr.Name;

            if (req.Query.ContainsKey(paramName))
            {
                SetPropertyValueFromQuery(prop, command!, req.Query[paramName]!);
            }
        }
    }

    private static Dictionary<string, object> ExtractRouteData(HttpRequest req)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // Extract route data from request context
        if (req.HttpContext.Items.TryGetValue("MSWebJobsRouter.TemplateBindings", out var templateBindings) &&
            templateBindings is IDictionary<string, object> bindings)
        {
            foreach (var binding in bindings)
            {
                if (binding.Value is not null)
                {
                    result[binding.Key] = binding.Value;
                }
            }
        }

        // Extract from Azure Functions route data
        if (req.RouteValues is not null)
        {
            foreach (var route in req.RouteValues)
            {
                result[route.Key] = route.Value!;
            }
        }

        return result;
    }

    private static void SetPropertyValue(PropertyInfo prop, object target, object value)
    {
        if (value is null)
        {
            return;
        }

        try
        {
            object convertedValue;

            if (prop.PropertyType == typeof(Guid))
            {
                if (value is string s)
                {
                    convertedValue = Guid.Parse(s);
                }
                else
                {
                    convertedValue = value;
                }
            }
            else if (prop.PropertyType.IsEnum)
            {
                convertedValue = Enum.Parse(prop.PropertyType, value.ToString()!);
            }
            // TODO: add other types if required
            else
            {
                convertedValue = Convert.ChangeType(value, prop.PropertyType);
            }

            prop.SetValue(target, convertedValue);
        }
        catch (Exception)
        {
            // Failed to convert - leave property as default
        }
    }

    private static void SetPropertyValueFromQuery(PropertyInfo prop, object target, string value)
    {
        if (string.IsNullOrEmpty(value)) return;

        // Handle collections
        if (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            var elementType = prop.PropertyType.GetGenericArguments()[0];
            var list = Activator.CreateInstance(prop.PropertyType);
            var addMethod = prop.PropertyType.GetMethod("Add");

            var values = value.Split(',');

            foreach (var item in values)
            {
                addMethod?.Invoke(list, [Convert.ChangeType(item, elementType)]);
            }

            prop.SetValue(target, list);
        }
        else
        {
            SetPropertyValue(prop, target, value);
        }
    }

    private static async Task ProcessBodyContent(HttpRequest req, object command, PropertyInfo[] properties)
    {
        var bodyProps = properties.Where(p => p.GetCustomAttribute<FromBodyAttribute>() is not null).ToList();

        if (bodyProps.Count != 0 && req.ContentType?.Contains("application/json") == true)
        {
            using var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();

            if (!string.IsNullOrEmpty(body))
            {
                try
                {
                    var bodyProp = bodyProps.FirstOrDefault(p => p.Name == "Body");

                    if (bodyProp is null)
                    {
                        var bodyObj = JsonSerializer.Deserialize<JsonElement>(body);

                        foreach (var prop in bodyProps)
                        {
                            if (bodyObj.TryGetProperty(prop.Name.ToLower(), out var element))
                            {
                                var rawText = element.GetRawText();

                                prop.SetValue(command, rawText.Deserialize(prop.PropertyType));
                            }
                        }
                    }
                    else
                    {
                        var bodyObj = body.Deserialize(bodyProp.PropertyType);

                        bodyProp.SetValue(command, bodyObj);
                    }
                }
                catch (JsonException)
                {
                    // JSON parsing failed - leave properties as default
                }
            }
        }
    }
}
