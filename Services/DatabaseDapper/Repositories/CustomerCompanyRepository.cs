
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
    => await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerCompany>(
        CustomerCompanyQueries.GetById, new { Id = id }, transaction: _transaction
    );

    public async Task<SqlResult?> InsertAsync(CustomerCompany entity )
    {
        try {
            return await _transaction.Connection.QuerySingleAsync<SqlResult>(
                CustomerCompanyQueries.Insert, entity, transaction: _transaction
            );
        } 
        catch(SqlException ex) when (ex.Message?.Contains("Cannot insert duplicate key") == true) { //(ex.Number == 2627) {
            throw new UniqueConstraintException("Cannot insert customer. Company Code already exists.");
        }
    }

    public async Task<bool> DeleteByIdAsync(int id )
    => (await _transaction.Connection.ExecuteAsync(CustomerCompanyQueries.Delete, new { Id = id }, transaction: _transaction)) > 0;
    

    public async Task<SqlResult> UpdateAsync(CustomerCompany entity )
    =>await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerCompanyQueries.Update, entity, transaction: _transaction);
}