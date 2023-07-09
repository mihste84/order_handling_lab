namespace AppLogic.Common.Interfaces;

public interface ICountryRepository
{
    Task<IEnumerable<Country>> SelectAll();
}