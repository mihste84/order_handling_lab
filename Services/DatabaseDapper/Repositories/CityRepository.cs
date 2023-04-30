
namespace Services.DatabaseDapper.Repositories;

public class CityRepository : ICityRepository
{
    private readonly IDbTransaction _transaction;

    public CityRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<City?> GetByIdAsync(int id )
    {
        var sql = "SELECT * FROM Cities WHERE Id = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<City>(sql, new { Id = id }, transaction: _transaction);
    }

    public async Task<City?> GetByNameAsync(string name )
    {
        var sql = "SELECT * FROM Cities WHERE Name = @Id";
        return await _transaction.Connection.QueryFirstOrDefaultAsync<City>(sql, new { Name = name }, transaction: _transaction);
    }

    public async Task<int?> InsertAsync(City entity )
    {
        var sql = """
            INSERT INTO Cities (Name, CountryId, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id] 
            VALUES (@Name, @CountryId, @CreatedBy, @UpdatedBy);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id )
    {
        var sql = "DELETE FROM Cities WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(City entity )
    {
        var sql = """
            UPDATE Cities 
            SET Name = @Name,
                CountryId = @CountryId,               
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }
}