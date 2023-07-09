namespace AppLogic.Common.Interfaces;

public interface ICustomerPersonRepository
{
    Task<CustomerPerson?> GetByIdAsync(int id );
    Task<SqlResult?> InsertAsync(CustomerPerson entity );
    Task<bool> DeleteByIdAsync(int id );
    Task<SqlResult> UpdateAsync(CustomerPerson entity );
}