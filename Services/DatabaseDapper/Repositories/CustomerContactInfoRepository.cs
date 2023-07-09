namespace Services.DatabaseDapper.Repositories;

public class CustomerContactInfoRepository  : ICustomerContactInfoRepository {

    private readonly IDbTransaction _transaction;

    public CustomerContactInfoRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<bool> DeleteByIdAsync(int id )
    => (await _transaction.Connection.ExecuteAsync(CustomerContactInfoQueries.Delete, new { Id = id }, _transaction))> 0;

    public async Task<CustomerContactInfo?> GetByIdAsync(int id )
    => await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerContactInfo>(
        CustomerContactInfoQueries.GetById, new { Id = id }, _transaction
    );

    public async Task<SqlResult?> InsertAsync(CustomerContactInfo entity)
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(
        CustomerContactInfoQueries.Insert, entity, _transaction
    ); 

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerContactInfo> contactInfo)
    =>  await _transaction.Connection.ExecuteAsync(CustomerContactInfoQueries.InsertMultiple, contactInfo, _transaction) > 0;

    public async Task<SqlResult> UpdateAsync(CustomerContactInfo entity )
    => await _transaction.Connection.QuerySingleAsync<SqlResult>(CustomerContactInfoQueries.Update, entity, _transaction);
}