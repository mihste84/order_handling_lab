namespace AppLogic.Common.Interfaces;


public interface ICustomerAddressRepository 
{
    Task<bool> InsertMultipleAsync(IEnumerable<CustomerAddress> addresses);
    Task RemoveAllPrimaryAsync(int? customerId);
    Task<CustomerAddress?> GetByIdAsync(int id );
    Task<SqlResult?> InsertAsync(CustomerAddress entity );
    Task<bool> DeleteByIdAsync(int id );
    Task<SqlResult> UpdateAsync(CustomerAddress entity );
    Task<(IEnumerable<City> Cities, IEnumerable<Country> Countries)> GetAllReferenceDataAsync();
}