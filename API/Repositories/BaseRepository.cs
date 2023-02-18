namespace API.Repositories;


public class BaseRepository : IAsyncDisposable
{
    protected readonly AppDbContext Context;

    public BaseRepository(IDbContextFactory<AppDbContext> factory)
    {
        Context = factory.CreateDbContext();
    }

    public ValueTask DisposeAsync() => Context.DisposeAsync();
}