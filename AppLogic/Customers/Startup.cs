using AppLogic.Customers.BaseCustomers.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AppLogic.Customers;

public static class Startup
{
    public static void AddCustomerServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(InsertCustomerCommand).Assembly);
        services.AddValidatorsFromAssemblyContaining<CustomerAddressModelValidator>();
    }
}
