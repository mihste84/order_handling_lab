using Microsoft.Extensions.DependencyInjection;

namespace AppLogic.Common;
public static class Startup
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehaviour<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }
}
