namespace Services.DatabaseDapper;

public static class Startup
{
    public static void AddDatabaseServices(this IServiceCollection services, string? connectionString)
    {
        services.AddScoped<IDbConnection>(provider => new SqlConnection(connectionString));
        services.AddScoped<IUnitOfWork, DapperUnitOfWork>();
    }
}