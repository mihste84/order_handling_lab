
using Models.Exceptions;

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

    public async Task<CustomerPerson?> GetByIdAsync(int id)
    {
        var sql = "SELECT * FROM CustomerPersons WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerPerson>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<SqlResult?> InsertAsync(CustomerPerson entity)
    {
        var sql = """
            INSERT INTO CustomerPersons (FirstName, MiddleName, LastName, Ssn, CustomerId, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            VALUES (@FirstName, @MiddleName, @LastName, @Ssn, @CustomerId, @CreatedBy, @UpdatedBy);
        """;
        try {
            return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, transaction: _transaction);
        } 
        catch(SqlException ex) when (ex.Message?.Contains("Cannot insert duplicate key") == true) { //(ex.Number == 2627) {
            throw new UniqueConstraintException("Cannot insert customer. SSN already exists.");
        }
    }

    public async Task<bool> DeleteByIdAsync(int id)
    {
        var sql = "DELETE FROM CustomerPersons WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<SqlResult> UpdateAsync(CustomerPerson entity)
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
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            WHERE Id = @Id
        """;
        return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, transaction: _transaction);
    }
}