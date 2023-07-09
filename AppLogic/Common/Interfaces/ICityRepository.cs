namespace AppLogic.Common.Interfaces;

public interface ICityRepository
{
    Task<IEnumerable<City>> SelectAll();
}