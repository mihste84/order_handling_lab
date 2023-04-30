namespace AppLogic.Common.Interfaces;


public interface ICustomerContactInfoRepository : IBaseRepository<CustomerContactInfo>
{
    public Task<bool> InsertMultipleAsync(IEnumerable<CustomerContactInfo> contactInfo);
    Task<IEnumerable<CustomerContactInfo>> GetByCustomerIdAsync(int? customerId );
}