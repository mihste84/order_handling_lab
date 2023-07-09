namespace Services.DatabaseDapper.Repositories;

public class CustomerAddressRepository : ICustomerAddressRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerAddressRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<bool> DeleteByIdAsync(int id )
    {
        var sql = "DELETE FROM CustomerAddresses WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, _transaction)) > 0;
    }

    public async Task<CustomerAddress?> GetByIdAsync(int id )
    {
        var sql = "SELECT * FROM CustomerAddresses WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerAddress>(sql, new { Id = id }, _transaction);
    }


    public async Task<SqlResult?> InsertAsync(CustomerAddress entity )
    {
        var sql = """
            INSERT INTO CustomerAddresses (Address, IsPrimary, PostArea, ZipCode, CountryId, CustomerId, CityId, CreatedBy, UpdatedBy)
            OUTPUT INSERTED.[Id], INSERTED.RowVersion
            VALUES (@Address, @IsPrimary, @PostArea, @ZipCode, @CountryId, @CustomerId, @CityId, @CreatedBy, @UpdatedBy);
        """;

        return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, _transaction);
    }

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerAddress> addresses)
    {
        var sql = """
            INSERT INTO CustomerAddresses (Address, IsPrimary, PostArea, ZipCode, CountryId, CustomerId, CityId, CreatedBy, UpdatedBy) 
            VALUES (@Address, @IsPrimary, @PostArea, @ZipCode, @CountryId, @CustomerId, @CityId, @CreatedBy, @UpdatedBy);
        """;

        return await _transaction.Connection.ExecuteAsync(sql, addresses, _transaction) > 0;
    }

    public async Task<SqlResult> UpdateAsync(CustomerAddress entity )
    {
        var sql = """
            UPDATE CustomerAddresses 
            SET Address = @Address,
                Primary = @Primary,
                PostArea = @PostArea,
                ZipCode = @ZipCode,
                CityId = @CityId,
                CountryId = @CountryId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            OUTPUT INSERTED.[Id], INSERTED.RowVersion
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteScalarAsync<SqlResult>(sql, entity, _transaction);
    }

    public async Task RemoveAllPrimaryAsync(int? customerId)
    {
        var sql = "UPDATE CustomerAddresses SET IsPrimary = 0 WHERE CustomerId = @CustomerId";
        await _transaction.Connection.QuerySingleAsync(sql, new { CustomerId = customerId }, _transaction);
    }

    public async Task<(IEnumerable<City> Cities, IEnumerable<Country> Countries)> GetAllReferenceData() {
        var sql = "SELECT * FROM Cities; SELECT * FROM Countries;";

        var mapper = await _transaction.Connection.QueryMultipleAsync(sql,_transaction); 
        
        return (
            await mapper.ReadAsync<City>(),
            await mapper.ReadAsync<Country>()
        );
    }
}