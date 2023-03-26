
namespace Services.DatabaseDapper.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Customers WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<Customer>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<Customer?> GetBySsnAsync(string ssn, CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT * FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            WHERE cp.Ssn = @Ssn
        """;
        var customer = await _transaction.Connection.QueryAsync<Customer, CustomerPerson, Customer>(
                sql,
                (c, cp) =>
                {
                    c.CustomerPerson = cp;
                    return c;
                },
                new { Ssn = ssn }, 
                transaction: _transaction
            );
        return customer?.FirstOrDefault(); 
    }

    public async Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        var sql = """
            SELECT * FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            WHERE cc.Code = @Code
        """;
        var customer = await _transaction.Connection.QueryAsync<Customer, CustomerCompany, Customer>(
                sql,
                (c, cc) =>
                {
                    c.CustomerCompany = cc;
                    return c;
                },
                new { Code = code }, 
                transaction: _transaction
            );
        return customer?.FirstOrDefault(); 
    }

    public async Task<int?> InsertAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            INSERT INTO Customers (Active, CreatedBy, UpdatedBy, Created, Updated) 
            OUTPUT INSERTED.[Id] 
            VALUES (@Active, @CreatedBy, @UpdatedBy, @Created, @Updated);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Customers WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(Customer entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            UPDATE Customers 
            SET Active = @Active,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }

    private async Task<Customer?> BaseQueryCustomerAsync(string sql, object param)
    {
        var customer = (await _transaction.Connection.QueryAsync<Customer, CustomerCompany, CustomerPerson, Customer>(
                sql,
                (c, cc, cp) =>
                {
                    c.CustomerCompany = cc;
                    c.CustomerPerson = cp;
                    return c;
                },
                param, 
                transaction: _transaction
            )).AsQueryable();
        return customer?.FirstOrDefault();  
    }
}