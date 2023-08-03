
using Models.Exceptions;

namespace DatabaseDapper.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IDbTransaction _transaction;

    public CustomerRepository(IDbTransaction transaction)
    {
        _transaction = transaction;
    }

    public async Task<Customer?> GetByIdAsync(int id, bool includeContactInfo = true)
    {
        var mapper = await _transaction.Connection.QueryMultipleAsync(
                CustomerQueries.GetByIdQuery(includeContactInfo),
                new { Id = id },
                _transaction
            );

        var customer = await mapper.ReadFirstOrDefaultAsync<Customer>();
        if (customer == null) return default;
        customer.CustomerPerson = await mapper.ReadFirstOrDefaultAsync<CustomerPerson>();
        customer.CustomerCompany = await mapper.ReadFirstOrDefaultAsync<CustomerCompany>();
        if (includeContactInfo)
        {
            customer.CustomerContactInfos = (await mapper.ReadAsync<CustomerContactInfo>()).ToList();
            customer.CustomerAddresses = (await mapper.ReadAsync<CustomerAddress>()).ToList();
        }

        return customer;
    }

    public async Task<SearchResult<Customer>> SearchCustomersAsync(DynamicSearchQuery query)
    {
        var customerQuery = _transaction.Connection.QueryBuilder(CustomerQueries.GetSearchCustomersQuery(query));

        foreach (var searchItem in query.SearchItems.Where(_ => _.HandleAutomatically))
        {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(searchItem);
            customerQuery.Where(sql);
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "Phone", out var item))
        {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(
                new SearchItem("Value", item!.Value, item.Operator, item.HandleAutomatically) // Overwrite name to "Value"
            );
            customerQuery.Where(
                $"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerContactInfo] WHERE {sql} AND Type = {ContactInfoType.Phone})"
            );
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "Email", out item))
        {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(
                new SearchItem("Value", item!.Value, item!.Operator, item!.HandleAutomatically) // Overwrite name to "Value" 
            );
            customerQuery.Where(
                $"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerContactInfo] WHERE {sql} AND Type = {ContactInfoType.Email})"
            );
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "CityId", out item))
        {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerAddresses] WHERE {sql})");
        }

        if (DynamicSearchQuery.TryExtractSearchItemByName(query.SearchItems, "CountryId", out item))
        {
            var sql = DynamicSearchQuery.GetWhereFromSearchItem(item!);
            customerQuery.Where($"c.[Id] IN (SELECT [CustomerId] FROM [dbo].[CustomerAddresses] WHERE {sql})");
        }

        var reader = await customerQuery
            .QueryMultipleAsync(transaction: _transaction);
        var data = await reader.ReadAsync();
        if (data == null) return new(0, Array.Empty<Customer>());

        var count = await reader.ReadFirstOrDefaultAsync<int>();

        return new SearchResult<Customer>(count, data!.Select(_ => new Customer
        {
            Active = _.Active,
            Created = _.Created,
            CreatedBy = _.CreatedBy,
            Id = _.Id,
            Updated = _.Updated,
            UpdatedBy = _.UpdatedBy,
            CustomerCompany = _.IsCompany == 1 ? new CustomerCompany
            {
                Code = _.Code,
                Name = _.Name
            } : null,
            CustomerPerson = _.IsPerson == 1 ? new CustomerPerson
            {
                FirstName = _.FirstName,
                LastName = _.LastName,
                MiddleName = _.MiddleName,
                Ssn = _.Ssn
            } : null
        }));
    }

    public async Task<Customer?> GetBySsnAsync(string ssn)
    {
        var mapper = await _transaction.Connection.QueryMultipleAsync(
                CustomerQueries.GetBySsn,
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

    public async Task<Customer?> GetByCodeAsync(string code)
    {
        var mapper = await _transaction.Connection.QueryMultipleAsync(
                CustomerQueries.GetByCode,
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

    public async Task<SqlResult?> InsertAsync(Customer entity)
    {
        var parameters = new DynamicParameters();
        var IsCompany = entity.CustomerCompany != null;
        parameters.Add("Active", entity.Active);
        parameters.Add("CreatedBy", entity.CreatedBy);
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        parameters.Add("IsCompany", IsCompany ? 1 : 0);
        if (IsCompany)
        {
            parameters.Add("Code", entity.CustomerCompany!.Code);
            parameters.Add("Name", entity.CustomerCompany!.Name);
            parameters.Add("FirstName", null);
            parameters.Add("LastName", null);
            parameters.Add("MiddleName", null);
            parameters.Add("Ssn", null);
        }
        else
        {
            parameters.Add("FirstName", entity.CustomerPerson!.FirstName);
            parameters.Add("LastName", entity.CustomerPerson!.LastName);
            parameters.Add("MiddleName", entity.CustomerPerson!.MiddleName);
            parameters.Add("Ssn", entity.CustomerPerson!.Ssn);
            parameters.Add("Code", null);
            parameters.Add("Name", null);
        }

        try
        {
            return await _transaction.Connection.QuerySingleAsync<SqlResult>(
                CustomerQueries.Insert, parameters, transaction: _transaction
            );
        }
        catch (SqlException ex) when (ex.Message?.Contains("Cannot insert duplicate key") == true)
        { //(ex.Number == 2627) {
            var message = IsCompany
                ? "Cannot insert customer company. Code already exists."
                : "Cannot insert customer. SSN already exists.";
            throw new UniqueConstraintException(message);
        }
    }

    public async Task<bool> DeleteByIdAsync(int id)
    => (await _transaction.Connection.ExecuteAsync(CustomerQueries.Delete, new { Id = id }, transaction: _transaction)) > 0;

    public async Task<SqlResult> UpdateAsync(Customer entity)
    {
        var parameters = new DynamicParameters();
        var IsCompany = entity.CustomerCompany != null;
        parameters.Add("Id", entity.Id);
        parameters.Add("Active", entity.Active);
        parameters.Add("CreatedBy", entity.CreatedBy);
        parameters.Add("UpdatedBy", entity.UpdatedBy);
        parameters.Add("Updated", entity.Updated);
        parameters.Add("IsCompany", IsCompany ? 1 : 0);
        if (IsCompany)
        {
            parameters.Add("Code", entity.CustomerCompany!.Code);
            parameters.Add("Name", entity.CustomerCompany!.Name);
            parameters.Add("FirstName", null);
            parameters.Add("LastName", null);
            parameters.Add("MiddleName", null);
            parameters.Add("Ssn", null);
        }
        else
        {
            parameters.Add("FirstName", entity.CustomerPerson!.FirstName);
            parameters.Add("LastName", entity.CustomerPerson!.LastName);
            parameters.Add("MiddleName", entity.CustomerPerson!.MiddleName);
            parameters.Add("Ssn", entity.CustomerPerson!.Ssn);
            parameters.Add("Code", null);
            parameters.Add("Name", null);
        }

        return await _transaction.Connection.QuerySingleAsync<SqlResult>(
            CustomerQueries.Update, parameters, transaction: _transaction
        );
    }
}