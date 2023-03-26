using Respawn;

namespace Tests.IntegrationTests.Repositories;

public class TestBase
{
    protected readonly IUnitOfWork _unitOfWork;

    public TestBase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    protected async static Task<Respawner> GetRespawnerAsync(string connectionString) => await Respawner.CreateAsync(
        connectionString,
        new RespawnerOptions {
            TablesToIgnore = new Respawn.Graph.Table[] { "Countries", "Cities" }
        }
    );

    protected async static Task ResetDatabaseAsync(string connectionString)
    {
        var respawn = await GetRespawnerAsync(connectionString);
        await respawn.ResetAsync(connectionString);
    }
}
