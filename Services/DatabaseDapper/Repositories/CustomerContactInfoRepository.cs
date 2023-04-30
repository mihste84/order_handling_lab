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

    public async Task<IEnumerable<CustomerContactInfo>> GetByCustomerIdAsync(int? customerId)
    {
        var sql = "SELECT * FROM CustomerContactInfo WHERE CustomerId = @CustomerId";
        return await _transaction.Connection.QueryAsync<CustomerContactInfo>(sql, new { CustomerId = customerId }, _transaction);
    }

    public async Task<int?> InsertAsync(CustomerContactInfo entity)
    {
        var sql = """
            INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id] 
            VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
        """;
        
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, _transaction);
    }

    public async Task<bool> InsertMultipleAsync(IEnumerable<CustomerContactInfo> contactInfo)
    {
        var sql = """
            INSERT INTO CustomerContactInfo (Type, Value, CustomerId, CreatedBy, UpdatedBy) 
            VALUES (@Type, @Value, @CustomerId, @CreatedBy, @UpdatedBy);
        """;

        return await _transaction.Connection.ExecuteAsync(sql, contactInfo, _transaction) > 0;
    }

    public async Task<bool> UpdateAsync(CustomerContactInfo entity )
    {
        var sql = """
            UPDATE CustomerContactInfo 
            SET Type = @Type,
                Value = @Value,
                CustomerId = @CustomerId,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, _transaction) > 0;
    }
}