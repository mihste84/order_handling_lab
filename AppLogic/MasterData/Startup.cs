using MasterData.BaseMasterData.Queries;
using Microsoft.Extensions.DependencyInjection;

namespace MasterData;

public static class Startup
{
    public static void AddMasterDataServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<SelectAllMasterDataQuery>());
    }
}
