using Microsoft.Extensions.DependencyInjection;
using AppLogic.Common;
using AppLogic.Customers;

namespace Tests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddCommonServices();
        services.AddCustomerServices();
        services.AddScoped<IAuthenticationService, TestAuthenticationService>();
    }
}