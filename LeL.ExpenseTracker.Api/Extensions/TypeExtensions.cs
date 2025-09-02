namespace LeL.ExpenseTracker.Api.Extensions;

public static class TypeExtensions
{
    public static bool HasGenericTypeInterface(this Type type, Type interfaceType)
    {
        return type.GetInterfaces().Any(t => t.IsGenericType(interfaceType));
    }
    
    public static bool IsGenericType(this Type type, Type genericType)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
    }
}