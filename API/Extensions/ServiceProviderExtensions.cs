using System.Reflection;

namespace API.Extensions;


public static class ServiceProviderExtensions
{
    public static IServiceCollection RegisterAllServices(this IServiceCollection services) {
        var types = Assembly.GetExecutingAssembly().GetTypes();
        var interfaceTypes = types
            .SelectMany(_ => _.GetInterfaces())
            .Where(x => x.Namespace == "API.Interfaces");

        foreach (var customInterface in interfaceTypes)
        {
            var implementedBy = types.FirstOrDefault(_ => _.IsAssignableTo(customInterface) && !_.IsInterface);
            if (implementedBy == null)
                throw new NotImplementedException($"Interface {customInterface.Name} is not implemented.");

            services.AddScoped(customInterface, implementedBy);
        }
        return services;
    }
}