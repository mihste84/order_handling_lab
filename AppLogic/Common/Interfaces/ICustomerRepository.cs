namespace Common.Interfaces;

public interface ICustomerRepository
{
    Task<Customer?> GetBySsnAsync(string ssn);
    Task<Customer?> GetByCodeAsync(string code);
    Task<SearchResult<Customer>> SearchCustomersAsync(DynamicSearchQuery query);
    Task<Customer?> GetByIdAsync(int id, bool includeContactInfo = true);
    Task<SqlResult?> InsertAsync(Customer entity);
    Task<bool> DeleteByIdAsync(int id);
    Task<SqlResult> UpdateAsync(Customer entity);
}