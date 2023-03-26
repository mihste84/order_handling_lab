using Microsoft.Extensions.DependencyInjection;
using Services.DatabaseDapper;

namespace Tests.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDatabaseServices(Environment.GetEnvironmentVariable("DB_CONNECTION_STRING"));
    }
}