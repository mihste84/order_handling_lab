
using Models.Exceptions;

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

    public async Task<CustomerCompany?> GetByIdAsync(int id )
    {
        var sql = "SELECT * FROM CustomerCompanies WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerCompany>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<SqlResult?> InsertAsync(CustomerCompany entity )
    {
        var sql = """
            INSERT INTO CustomerCompanies (Code, Name, CustomerId, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            VALUES (@Code, @Name, @CustomerId, @CreatedBy, @UpdatedBy);
        """;
        try {
            return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, transaction: _transaction);
        } 
        catch(SqlException ex) when (ex.Message?.Contains("Cannot insert duplicate key") == true) { //(ex.Number == 2627) {
            throw new UniqueConstraintException("Cannot insert customer. Company Code already exists.");
        }
    }

    public async Task<bool> DeleteByIdAsync(int id )
    {
        var sql = "DELETE FROM CustomerCompanies WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction)) > 0;
    }

    public async Task<SqlResult> UpdateAsync(CustomerCompany entity )
    {
        var sql = """
            UPDATE CustomerCompanies 
            SET Code = @Code,
                Name = @Name,
                CustomerId = @CustomerId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            WHERE Id = @Id
        """;
        return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, transaction: _transaction);
    }
}