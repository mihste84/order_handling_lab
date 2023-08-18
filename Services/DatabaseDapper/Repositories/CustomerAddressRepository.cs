namespace DatabaseDapper.Repositories;

public class CustomerAddressRepository : ICustomerAddressRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerAddressRepository(IDbTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task<bool> DeleteByIdAsync(int id)
    => (await _transaction.Connection.ExecuteAsync(CustomerAddressQueries.Delete, new { Id = id }, _transaction)) > 0;

    public async Task<CustomerAddress?> GetByIdAsync(int id)
    => await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerAddress>(
        CustomerAddressQueries.GetById, new { Id = id }, _transaction
    );

    public async Task<int?> GetCountByCustomerIdAsync(int id)
    => await _transaction.Connection.QueryFirstOrDefaultAsync<int>(
        CustomerAddressQueries.GetCountByCustomerId, new { CustomerId = id }, _transaction
    );

    public async Task<IEnumerable<CustomerAddress>?> GetByCustomerIdAsync(int id)
    => await _transaction.Connection.QueryAsync<CustomerAddress>(
        CustomerAddressQueries.GetByCustomerId, new { CustomerId = id }, _transaction
    );

    public async Task<SqlResult?> InsertAsync(CustomerAddress entity)
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerAddressQueries.Insert, entity, _transaction);

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerAddress> addresses)
    => await _transaction.Connection.ExecuteAsync(CustomerAddressQueries.InsertMultiple, addresses, _transaction) > 0;

    public async Task<SqlResult> UpdateAsync(CustomerAddress entity)
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerAddressQueries.Update, entity, _transaction);

    public async Task RemoveAllPrimaryAsync(int? customerId)
    => await _transaction.Connection.ExecuteAsync(
        CustomerAddressQueries.RemoveAllPrimary, new { CustomerId = customerId }, _transaction
    );

    public async Task<(IEnumerable<City> Cities, IEnumerable<Country> Countries)> GetAllReferenceDataAsync()
    {
        var mapper = await _transaction.Connection.QueryMultipleAsync(CustomerAddressQueries.GetAllReferenceDataAsync, transaction: _transaction);

        return (
            await mapper.ReadAsync<City>(),
            await mapper.ReadAsync<Country>()
        );
    }
}