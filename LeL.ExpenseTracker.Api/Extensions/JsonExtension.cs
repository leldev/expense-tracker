using System.Text.Json;
using System.Text.Json.Serialization;

namespace LeL.ExpenseTracker.Api.Extensions;

public static class JsonExtension
{
    public static readonly JsonSerializerOptions DefaultOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        WriteIndented = true,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true),
        },
    };

    public static T Deserialize<T>(this string content) => JsonSerializer.Deserialize<T>(content, DefaultOptions)!;

    public static object Deserialize(this string content, Type type) => JsonSerializer.Deserialize(content, type, DefaultOptions)!;

    public static string Serialize<T>(this T content) => JsonSerializer.Serialize(content, DefaultOptions);
}
