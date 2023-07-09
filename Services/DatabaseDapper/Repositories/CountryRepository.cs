
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

    public async Task<IEnumerable<Country>> SelectAll() {
        var sql = "SELECT * FROM Countries";
        return await _transaction.Connection.QueryAsync<Country>(sql, transaction: _transaction);
    }
}