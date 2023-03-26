
namespace Services.DatabaseDapper.Repositories;

public class CountryRepository : ICountryRepository
{
    private readonly IDbTransaction _transaction;

    public CountryRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<Country?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Countries WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<Country>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<Country?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var sql = "SELECT * FROM Countries WHERE Name = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<Country>(sql, new { Name = name }, transaction: _transaction);
    }

    public async Task<int?> InsertAsync(Country entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            INSERT INTO Countries (Name, Abbreviation, PhonePrefix, CreatedBy, UpdatedBy, Created, Updated) 
            OUTPUT INSERTED.[Id] 
            VALUES (@Name, @Abbreviation, @PhonePrefix, @CreatedBy, @UpdatedBy, @Created, @Updated);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var sql = "DELETE FROM Countries WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(Country entity, CancellationToken cancellationToken = default)
    {
        var sql = """
            UPDATE Countries 
            SET Name = @Name,
                Abbreviation = @Abbreviation,
                PhonePrefix = @PhonePrefix,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }
}