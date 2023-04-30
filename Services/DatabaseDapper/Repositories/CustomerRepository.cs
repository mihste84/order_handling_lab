
using Models.Constants;
using Models.Values;

namespace Services.DatabaseDapper.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerRepository(IDbTransaction? transaction)
    {
        if (transaction == null)
            throw new ArgumentNullException(nameof(transaction));

        _transaction = transaction;
    }

    public async Task<Customer?> GetByIdAsync(int id)
    {
        var sql = """
            SELECT TOP 1 c.* FROM Customers c
            WHERE c.Id = @Id

            SELECT TOP 1 cp.* FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            WHERE c.Id = @Id

            SELECT TOP 1 cc.* FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            WHERE c.Id = @Id

            SELECT cc.* FROM Customers c
            INNER JOIN CustomerContactInfo cc ON c.Id = cc.CustomerId
            WHERE c.Id = @Id

            SELECT ca.* FROM Customers c
            INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
            WHERE c.Id = @Id
        """;

        var mapper = await _transaction.Connection.QueryMultipleAsync(
                sql,
                new { Id = id }, 
                _transaction
            );
        
        var customer = await mapper.ReadFirstOrDefaultAsync<Customer>();
        if (customer == null) return default;
        customer.CustomerPerson = await mapper.ReadFirstOrDefaultAsync<CustomerPerson>();
        customer.CustomerCompany = await mapper.ReadFirstOrDefaultAsync<CustomerCompany>();
        customer.CustomerContactInfos = (await mapper.ReadAsync<CustomerContactInfo>()).ToList();
        customer.CustomerAddresses = (await mapper.ReadAsync<CustomerAddress>()).ToList();
        return customer;
    }

    public async Task<Customer?> GetBySsnAsync(string ssn )
    {
        var sql = """
            SELECT TOP 1 c.* FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            WHERE cp.Ssn = @Ssn

            SELECT TOP 1 cp.* FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            WHERE cp.Ssn = @Ssn

            SELECT cc.* FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            INNER JOIN CustomerContactInfo cc ON c.Id = cc.CustomerId
            WHERE cp.Ssn = @Ssn

            SELECT ca.* FROM Customers c
            INNER JOIN CustomerPersons cp ON c.Id = cp.CustomerId
            INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
            WHERE cp.Ssn = @Ssn
        """;

        var mapper = await _transaction.Connection.QueryMultipleAsync(
                sql,
                new { Ssn = ssn }, 
                _transaction
            );
        
        var customer = await mapper.ReadFirstOrDefaultAsync<Customer>();
        if (customer == null) return default;
        customer.CustomerPerson = await mapper.ReadFirstOrDefaultAsync<CustomerPerson>();
        customer.CustomerContactInfos = (await mapper.ReadAsync<CustomerContactInfo>()).ToList();
        customer.CustomerAddresses = (await mapper.ReadAsync<CustomerAddress>()).ToList();
        return customer;
    }

    public async Task<SearchResult<Customer>> SearchCustomersAsync(DynamicSearchQuery query) 
    {
        var customerQuery = _transaction.Connection.QueryBuilder($@"
            SELECT * FROM (
                SELECT 
                    ROW_NUMBER() OVER (ORDER BY c.Id ASC) AS RowNumber
                    ,c.[Id] 
                    ,FirstName
                    ,LastName
                    ,MiddleName
                    ,Ssn
                    ,Code
                    ,Name
                    ,[Active]
                    ,c.[CreatedBy]
                    ,c.[Created]
                    ,c.[UpdatedBy]
                    ,c.[Updated]
                    ,CASE WHEN co.[Id] IS NOT NULL THEN 1 ELSE 0 END AS IsCompany
                    ,CASE WHEN cp.[Id] IS NOT NULL THEN 1 ELSE 0 END AS IsPerson
                FROM [dbo].[Customers] c
                LEFT JOIN dbo.CustomerCompanies co ON c.[Id] = co.[CustomerId]
                LEFT JOIN dbo.CustomerPersons cp ON c.[Id] = cp.[CustomerId]
                /**where**/
            ) x
            WHERE RowNumber between {query.StartRow} and {query.EndRow}    
            ORDER BY {query.OrderBy:raw} {query.OrderByDirection:raw};

            SELECT count(*) FROM [dbo].[Customers] c
            LEFT JOIN dbo.CustomerCompanies co ON c.[Id] = co.[CustomerId]
            LEFT JOIN dbo.CustomerPersons cp ON c.[Id] = cp.[CustomerId]
            /**where**/    
        ");
        
        foreach(var searchItem in query.SearchItems.Where(_ => _.IsBaseWhere)) {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(searchItem);
            customerQuery.Where(sql);
        }
            
        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "Phone", out var item)) {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerContactInfo] WHERE {sql} AND Type = {ContactInfoType.Phone})");
        }
            
        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "Email", out item)) {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerContactInfo] WHERE {sql} AND Type = {ContactInfoType.Email})");
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "CityId", out item)) {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerAddresses] WHERE {sql})");
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "CountryId", out item)) {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerAddresses] WHERE {sql})");
        }

        var reader = await customerQuery
            .QueryMultipleAsync(transaction: _transaction);
        var data = await reader.ReadAsync();
        if (data == null) return new(0, Array.Empty<Customer>());

        var count = await reader.ReadFirstOrDefaultAsync<int>();

        return new SearchResult<Customer>(count, data!.Select(_ => new Customer {
            Active = _.Active,
            Created = _.Created,
            CreatedBy = _.CreatedBy,
            Id = _.Id,
            Updated = _.Updated,
            UpdatedBy = _.UpdatedBy,
            CustomerCompany = _.IsCompany == 1 ? new CustomerCompany {
                Code = _.Code,
                Name = _.Name
            } : null,
            CustomerPerson = _.IsPerson == 1 ? new CustomerPerson {
                FirstName = _.FirstName,
                LastName = _.LastName,
                MiddleName = _.MiddleName
            } : null
        }));
    }

    public async Task<Customer?> GetByCodeAsync(string code )
    {
        var sql = """
            SELECT TOP 1 c.* FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            WHERE cc.Code = @Code

            SELECT TOP 1 cc.* FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            WHERE cc.Code = @Code

            SELECT cci.* FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            INNER JOIN CustomerContactInfo cci ON c.Id = cci.CustomerId
            WHERE cc.Code = @Code

            SELECT ca.* FROM Customers c
            INNER JOIN CustomerCompanies cc ON c.Id = cc.CustomerId
            INNER JOIN CustomerAddresses ca ON c.Id = ca.CustomerId
            WHERE cc.Code = @Code
        """;

        var mapper = await _transaction.Connection.QueryMultipleAsync(
                sql,
                new { Code = code }, 
                _transaction
            );
        
        var customer = await mapper.ReadFirstOrDefaultAsync<Customer>();
        if (customer == null) return default;
        customer.CustomerCompany = await mapper.ReadFirstOrDefaultAsync<CustomerCompany>();
        customer.CustomerContactInfos = (await mapper.ReadAsync<CustomerContactInfo>()).ToList();
        customer.CustomerAddresses = (await mapper.ReadAsync<CustomerAddress>()).ToList();
        return customer;
    }

    public async Task<int?> InsertAsync(Customer entity )
    {
        var sql = """
            INSERT INTO Customers (Active, CreatedBy, UpdatedBy) 
            OUTPUT INSERTED.[Id] 
            VALUES (@Active, @CreatedBy, @UpdatedBy);
        """;
        return await _transaction.Connection.ExecuteScalarAsync<int>(sql, entity, transaction: _transaction);
    }

    public async Task<bool> DeleteByIdAsync(int id )
    {
        var sql = "DELETE FROM Customers WHERE Id = @Id";
        return (await _transaction.Connection.ExecuteAsync(sql, new { Id = id }, transaction: _transaction))> 0;
    }

    public async Task<bool> UpdateAsync(Customer entity )
    {
        var sql = """
            UPDATE Customers 
            SET Active = @Active,
                UpdatedBy = @UpdatedBy,
                Updated = @Updated
            WHERE Id = @Id
        """;
        return await _transaction.Connection.ExecuteAsync(sql, entity, transaction: _transaction) > 0;
    }

    private async Task<Customer?> BaseQueryCustomerAsync(string sql, object param)
    {
        var customer = (await _transaction.Connection.QueryAsync<Customer, CustomerCompany, CustomerPerson, Customer>(
                sql,
                (c, cc, cp) =>
                {
                    c.CustomerCompany = cc;
                    c.CustomerPerson = cp;
                    return c;
                },
                param, 
                transaction: _transaction
            )).AsQueryable();
        return customer?.FirstOrDefault();  
    }
}