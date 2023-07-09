
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
    =>await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerPerson>(
        CustomerPersonQueries.GetById, new { Id = id }, transaction: _transaction
    );
    

    public async Task<SqlResult?> InsertAsync(CustomerPerson entity)
    {
        try {
            return await _transaction.Connection.QuerySingleAsync<SqlResult>(
                CustomerPersonQueries.Insert, entity, transaction: _transaction
            );
        } 
        catch(SqlException ex) when (ex.Message?.Contains("Cannot insert duplicate key") == true) { //(ex.Number == 2627) {
            throw new UniqueConstraintException("Cannot insert customer. SSN already exists.");
        }
    }

    public async Task<bool> DeleteByIdAsync(int id)
    => (await _transaction.Connection.ExecuteAsync(CustomerPersonQueries.Delete, new { Id = id }, transaction: _transaction))> 0;

    public async Task<SqlResult> UpdateAsync(CustomerPerson entity)
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerPersonQueries.Update, entity, transaction: _transaction);
}