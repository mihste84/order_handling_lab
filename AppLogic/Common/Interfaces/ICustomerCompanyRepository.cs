namespace AppLogic.Common.Interfaces;

public interface ICustomerCompanyRepository
{
    Task<CustomerCompany?> GetByIdAsync(int id );
    Task<SqlResult?> InsertAsync(CustomerCompany entity );
    Task<bool> DeleteByIdAsync(int id );
    Task<SqlResult> UpdateAsync(CustomerCompany entity );
}