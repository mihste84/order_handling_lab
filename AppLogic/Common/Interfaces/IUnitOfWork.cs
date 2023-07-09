namespace AppLogic.Common.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    ICustomerCompanyRepository CustomerCompanyRepository { get; }
    ICustomerPersonRepository CustomerPersonRepository { get; }
    ICustomerContactInfoRepository CustomerContactInfoRepository { get; }
    ICustomerAddressRepository CustomerAddressRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
