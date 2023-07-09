
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

    public async Task<IEnumerable<City>> SelectAll() {
        var sql = "SELECT * FROM Cities";
        return await _transaction.Connection.QueryAsync<City>(sql, transaction: _transaction);
    }

    
}