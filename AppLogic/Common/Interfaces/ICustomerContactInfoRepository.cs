namespace Common.Interfaces;

public interface ICustomerContactInfoRepository
{
    public Task<bool> InsertMultipleAsync(IEnumerable<CustomerContactInfo> contactInfo);
    Task<CustomerContactInfo?> GetByIdAsync(int id);
    Task<IEnumerable<CustomerContactInfo>?> GetByCustomerIdAsync(int id);
    Task<SqlResult?> InsertAsync(CustomerContactInfo entity);
    Task<bool> DeleteByIdAsync(int id);
    Task<SqlResult> UpdateAsync(CustomerContactInfo entity);
    Task<int?> GetCountByCustomerIdAsync(int id);
}