namespace AppLogic.Common.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetBySsnAsync(string ssn );
    Task<Customer?> GetByCodeAsync(string code );
    Task<SearchResult<Customer>> SearchCustomersAsync(DynamicSearchQuery query);
}