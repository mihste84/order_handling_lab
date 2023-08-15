using Microsoft.Extensions.DependencyInjection;
using Customers;
using Common;

namespace UnitTests;

public static class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddCommonServices();
        services.AddCustomerServices();
        services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    }
}