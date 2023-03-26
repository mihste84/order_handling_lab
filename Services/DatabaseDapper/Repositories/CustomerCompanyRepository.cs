
namespace Services.DatabaseDapper.Repositories;

public class CustomerCompanyRepository : ICustomerCompanyRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerCompanyRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<CustomerCompany?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM CustomerCompanies WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerCompany>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<int?> InsertAsync(CustomerCompany entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            INSERT INTO CustomerCompanies (Code, Name, CustomerId, CreatedBy, UpdatedBy, Created, Updated) 
            OUTPUT INSERTED.[Id]
            VALUES (@Code, @Name, @CustomerId, @CreatedBy, @UpdatedBy, @Created, @Updated);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM CustomerCompanies WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(CustomerCompany entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            UPDATE CustomerCompanies 
            SET Code = @Code,
                Name = @Name,
                CustomerId = @CustomerId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }
}