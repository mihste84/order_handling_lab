namespace AppLogic.Common.Interfaces;

public interface ICityRepository : IBaseRepository<City>
{
    Task<City?> GetByNameAsync(string name );
}