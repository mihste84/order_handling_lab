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

    public async Task<SqlResult?> InsertAsync(CustomerAddress entity)
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerAddressQueries.Insert, entity, _transaction);

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerAddress> addresses)
    => await _transaction.Connection.ExecuteAsync(CustomerAddressQueries.InsertMultiple, addresses, _transaction) > 0;

    public async Task<SqlResult> UpdateAsync(CustomerAddress entity)
    => await _transaction.Connection.ExecuteScalarAsync<SqlResult>(CustomerAddressQueries.Update, entity, _transaction);

    public async Task RemoveAllPrimaryAsync(int? customerId)
    => await _transaction.Connection.QuerySingleAsync(
        CustomerAddressQueries.RemoveAllPrimary, new { CustomerId = customerId }, _transaction
    );

    public async Task<(IEnumerable<City> Cities, IEnumerable<Country> Countries)> GetAllReferenceDataAsync()
    {
        var mapper = await _transaction.Connection.QueryMultipleAsync(CustomerAddressQueries.GetAllReferenceDataAsync, _transaction);

        return (
            await mapper.ReadAsync<City>(),
            await mapper.ReadAsync<Country>()
        );
    }
}