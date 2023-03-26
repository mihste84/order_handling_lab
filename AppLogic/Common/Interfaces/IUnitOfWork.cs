namespace AppLogic.Common.Interfaces;

public interface IUnitOfWork
{
    ICountryRepository CountryRepository { get; }
    ICityRepository CityRepository { get; }
    ICustomerRepository CustomerRepository { get; }
    ICustomerCompanyRepository CustomerCompanyRepository { get; }
    ICustomerPersonRepository CustomerPersonRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
