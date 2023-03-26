namespace AppLogic.Common.Interfaces;

public interface ICustomerRepository : IBaseRepository<Customer>
{
    Task<Customer?> GetBySsnAsync(string ssn, CancellationToken cancellationToken = default);
    Task<Customer?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}