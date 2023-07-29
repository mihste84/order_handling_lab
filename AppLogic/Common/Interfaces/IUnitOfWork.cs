namespace Common.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository CustomerRepository { get; }
    ICustomerContactInfoRepository CustomerContactInfoRepository { get; }
    ICustomerAddressRepository CustomerAddressRepository { get; }
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
