﻿using Microsoft.Extensions.DependencyInjection;

namespace Common;
public static class Startup
{
    public static void AddCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AuthenticationBehaviour<,>));
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehaviour<,>));
    }
}
