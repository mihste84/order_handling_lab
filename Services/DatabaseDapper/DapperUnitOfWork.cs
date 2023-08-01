using Microsoft.Extensions.Logging;

namespace DatabaseDapper;

public class DapperUnitOfWork : IUnitOfWork, IDisposable
{
    private bool _disposed;
    private IDbConnection _connection;
    private IDbTransaction _transaction;
    private readonly ILogger<DapperUnitOfWork> _logger;

    private ICustomerRepository? _customerRepository;
    public ICustomerRepository CustomerRepository => _customerRepository ??= new CustomerRepository(_transaction);

    private ICustomerContactInfoRepository? _customerContactInfoRepository;
    public ICustomerContactInfoRepository CustomerContactInfoRepository => _customerContactInfoRepository ??= new CustomerContactInfoRepository(_transaction);

    private ICustomerAddressRepository? _customerAddressRepository;
    public ICustomerAddressRepository CustomerAddressRepository => _customerAddressRepository ??= new CustomerAddressRepository(_transaction);
    public DapperUnitOfWork(IDbConnection connection, ILogger<DapperUnitOfWork> logger)
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
        catch (SqlException ex)
        {
            LogAllSqlExceptions(ex);
            _transaction?.Rollback();
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unknown error occurred while saving changes");
            _transaction?.Rollback();
            throw;
        }
        finally
        {
            _transaction?.Dispose();
            _transaction = _connection.BeginTransaction();
            ClearRepositories();
        }

        return Task.CompletedTask;
    }

    private void LogAllSqlExceptions(Exception ex)
    {
        _logger?.LogError(ex, "A database error occurred while saving changes.");
        if (ex.InnerException != null)
            LogAllSqlExceptions(ex.InnerException);
    }

    private void ClearRepositories()
    {
        _customerRepository = null;
        _customerContactInfoRepository = null;
        _customerAddressRepository = null;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed)
            return;

        if (disposing)
        {
            _transaction?.Dispose();
            _transaction = null!;
            _connection?.Dispose();
            _connection = null!;
        }

        _disposed = true;
    }
}