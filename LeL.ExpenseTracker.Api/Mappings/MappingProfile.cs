using AutoMapper;
using LeL.ExpenseTracker.Api.Extensions;

namespace LeL.ExpenseTracker.Api.Mappings;

public sealed class MappingProfile : Profile
{
    public MappingProfile()
    {
        ApplyMappingsFromAssembly();
    }

    private void ApplyMappingsFromAssembly()
    {
        var mapFromType = typeof(IMappable<>);
        var createMapMethodName = nameof(IMappable<object>.CreateMap);

        var types = typeof(Program).Assembly.GetExportedTypes().Where(t => t.HasGenericTypeInterface(mapFromType)).ToList();

        var argumentTypes = new Type[] { typeof(Profile) };

        foreach (var type in types)
        {
            var instance = Activator.CreateInstance(type);

            var methodInfo = type.GetMethod(createMapMethodName);

            if (methodInfo is null)
            {
                var interfaces = type.GetInterfaces().Where(t => t.IsGenericType(mapFromType)).ToList();

                if (interfaces.Count > 0)
                {
                    foreach (var intf in interfaces)
                    {
                        var interfaceMethodInfo = intf.GetMethod(createMapMethodName, argumentTypes);

                        interfaceMethodInfo?.Invoke(instance, [this]);
                    }
                }
            }
            else
            {
                methodInfo.Invoke(instance, [this]);
            }
        }
    }
}