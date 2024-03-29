using Microsoft.Extensions.DependencyInjection;
using Customers;
using Common;

namespace Tests;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddCommonServices();
        services.AddCustomerServices();
        services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    }
}