
namespace Services.DatabaseDapper.Repositories;

public class CustomerPersonRepository : ICustomerPersonRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerPersonRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<CustomerPerson?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = "SELECT * FROM CustomerPersons WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerPerson>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<int?> InsertAsync(CustomerPerson entity, CancellationToken cancellationToken)
    {
        var sql = """
            INSERT INTO CustomerPersons (FirstName, MiddleName, LastName, Ssn, CustomerId, CreatedBy, UpdatedBy, Created, Updated) 
            OUTPUT INSERTED.[Id] 
            VALUES (@FirstName, @MiddleName, @LastName, @Ssn, @CustomerId, @CreatedBy, @UpdatedBy, @Created, @Updated);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken)
    {
        var sql = "DELETE FROM CustomerPersons WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(CustomerPerson entity, CancellationToken cancellationToken)
    {
        var sql = """
            UPDATE CustomerPersons 
            SET FirstName = @FirstName,
                MiddleName = @MiddleName,
                LastName = @LastName,
                Ssn = @Ssn,
                CustomerId = @CustomerId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }
}