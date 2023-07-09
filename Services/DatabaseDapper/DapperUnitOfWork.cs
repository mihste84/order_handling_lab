using Microsoft.Extensions.Logging;

namespace Services.DatabaseDapper;

public class DapperUnitOfWork : IUnitOfWork, IDisposable
{
    private IDbConnection? _connection;
    private IDbTransaction? _transaction;
    private ILogger<DapperUnitOfWork>? _logger;

    private ICountryRepository? _countryRepository;
    public ICountryRepository CountryRepository => _countryRepository ?? (_countryRepository = new CountryRepository(_transaction));

    private ICityRepository? _cityRepository;
    public ICityRepository CityRepository => _cityRepository ?? (_cityRepository = new CityRepository(_transaction));

    private ICustomerRepository? _customerRepository;
    public ICustomerRepository CustomerRepository => _customerRepository ?? (_customerRepository = new CustomerRepository(_transaction));

    private ICustomerCompanyRepository? _customerCompanyRepository;
    public ICustomerCompanyRepository CustomerCompanyRepository => _customerCompanyRepository ?? (_customerCompanyRepository = new CustomerCompanyRepository(_transaction));

    private ICustomerPersonRepository? _customerPersonRepository;
    public ICustomerPersonRepository CustomerPersonRepository => _customerPersonRepository ?? (_customerPersonRepository = new CustomerPersonRepository(_transaction));

    private ICustomerContactInfoRepository? _customerContactInfoRepository;
    public ICustomerContactInfoRepository CustomerContactInfoRepository => _customerContactInfoRepository ?? (_customerContactInfoRepository = new CustomerContactInfoRepository(_transaction));

    private ICustomerAddressRepository? _customerAddressRepository;
    public ICustomerAddressRepository CustomerAddressRepository => _customerAddressRepository ?? (_customerAddressRepository = new CustomerAddressRepository(_transaction));
    public DapperUnitOfWork(IDbConnection connection, ILogger<DapperUnitOfWork>? logger)
    {
        _connection = connection;
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _logger = logger;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try 
        {
            _transaction?.Commit(); 
        } 
        catch(SqlException ex)
        { 
            LogAllSqlExceptions(ex);
            _transaction?.Rollback(); 
            throw; 
        } 
        catch(Exception ex)
        { 
            _logger?.LogError(ex, "An unknown error occurred while saving changes");
            _transaction?.Rollback(); 
            throw; 
        } 
        finally 
        { 
            _transaction?.Dispose(); 
            _transaction = _connection?.BeginTransaction(); 
            ResetRepositories(); 
        }

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _transaction = null;
        _connection?.Dispose();
        _connection = null;
    }

    private void LogAllSqlExceptions(Exception ex) 
    {
        _logger?.LogError(ex, "A database error occurred while saving changes.");
        if (ex.InnerException != null)
            LogAllSqlExceptions(ex.InnerException);
    }

    private void ResetRepositories()
    {
        _countryRepository = null;
        _cityRepository = null;
        _customerRepository = null;
        _customerCompanyRepository = null;
        _customerPersonRepository = null;
        _customerContactInfoRepository = null;
    }
}