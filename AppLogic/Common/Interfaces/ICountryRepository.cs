namespace AppLogic.Common.Interfaces;

public interface ICountryRepository : IBaseRepository<Country>
{
    Task<Country?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
}