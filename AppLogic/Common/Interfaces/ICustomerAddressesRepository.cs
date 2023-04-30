namespace AppLogic.Common.Interfaces;


public interface ICustomerAddressesRepository : IBaseRepository<CustomerAddress>
{
    Task<bool> InsertMultipleAsync(IEnumerable<CustomerAddress> addresses);
    Task<IEnumerable<CustomerAddress>> GetByCustomerIdAsync(int? customerId);
    Task RemoveAllPrimaryAsync(int? customerId);
}