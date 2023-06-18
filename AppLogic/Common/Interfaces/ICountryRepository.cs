namespace AppLogic.Common.Interfaces;

public interface ICountryRepository : IBaseRepository<Country>
{
    Task<Country?> GetByNameAsync(string name );
    Task<IEnumerable<Country>> SelectAll();
}