using AppLogic.Common;
using AppLogic.Customers;
using Authentication;
using ConsoleLab;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Services.DatabaseDapper;

using IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((ctx, config) =>
    {
        config.AddJsonFile("appsettings.json", false, true);
        config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.json", true, true);
        config.AddEnvironmentVariables();
    })
    .ConfigureServices((ctx, services) =>
    {
        services.AddDatabaseServices(ctx.Configuration.GetConnectionString("AppDbContext"));
        services.AddCommonServices();
        services.AddCustomerServices();
        services.AddTestAuthenticationServices();
        services.AddScoped<DatabaseLab>();
    })
    .UseConsoleLifetime()
    .Build();


using var scope = host.Services.CreateScope();;
var services = scope.ServiceProvider;
await services.GetRequiredService<DatabaseLab>().RunAsync();

// await host.RunAsync();

