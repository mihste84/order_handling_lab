namespace DatabaseDapper;

public static class Startup
{
    public static void AddDatabaseServices(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<IDbConnection>(_ => new SqlConnection(connectionString));
        services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
    }
}