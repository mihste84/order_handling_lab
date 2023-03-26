using AppLogic.Common.Interfaces;
using Services.Authentication;
using Microsoft.Extensions.DependencyInjection;

namespace Authentication;

public static class Startup
{
    public static void AddAuthenticationServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
    }
}
