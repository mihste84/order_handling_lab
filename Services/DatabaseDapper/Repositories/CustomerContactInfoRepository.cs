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
    {
        var sql = "DELETE FROM CustomerContactInfo WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, _transaction))> 0;
    }

    public async Task<CustomerContactInfo?> GetByIdAsync(int id )
    {
        var sql = "SELECT * FROM CustomerContactInfo WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<CustomerContactInfo>(sql, new { Id = id }, _transaction);
    }

    public async Task<SqlResult?> InsertAsync(CustomerContactInfo entity)
    {
        var sql = """
            INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id], INSERTED.RowVersion
            VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
        """;
        
        return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, _transaction);
    }

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerContactInfo> contactInfo)
    {
        var sql = """
            INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy)
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
        """;

        return await _transaction.Connection.ExecuteAsync(sql, contactInfo, _transaction) > 0;
    }

    public async Task<SqlResult> UpdateAsync(CustomerContactInfo entity )
    {
        var sql = """
            UPDATE CustomerContactInfo 
            SET Type = @Type,
                Value = @Value,
                CustomerId = @CustomerId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            OUTPUT INSERTED.[Id], INSERTED.RowVersion 
            WHERE Id = @Id
        """;
        return await _transaction.Connection.QuerySingleAsync<SqlResult>(sql, entity, _transaction);
    }
}