using Customers.BaseCustomers.Queries;
using Customers.Constants;

namespace Tests.Queries;

public class GetCustomerByValueQueryTests
{
    [Fact]
    public async Task GetCustomerByValueQuery_Id()
    {
        var mockUnitOfWork = GetMockUnitOfWork();
        var handler = new GetCustomerByValueQuery.GetCustomerByValueHandler(
            mockUnitOfWork.Object,
            new GetCustomerByValueQuery.GetCustomerByValueValidator()
        );
        var query = new GetCustomerByValueQuery { Type = CustomerSearchValues.Id, Value = "1" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetBySsnAsync(It.IsAny<string>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByCodeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomerByValueQuery_SSN()
    {
        var mockUnitOfWork = GetMockUnitOfWork();
        var handler = new GetCustomerByValueQuery.GetCustomerByValueHandler(
            mockUnitOfWork.Object,
            new GetCustomerByValueQuery.GetCustomerByValueValidator()
        );
        var query = new GetCustomerByValueQuery { Type = CustomerSearchValues.Ssn, Value = "12375654-1234" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetBySsnAsync(It.IsAny<string>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByCodeAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task GetCustomerByValueQuery_Code()
    {
        var mockUnitOfWork = GetMockUnitOfWork();
        var handler = new GetCustomerByValueQuery.GetCustomerByValueHandler(
            mockUnitOfWork.Object,
            new GetCustomerByValueQuery.GetCustomerByValueValidator()
        );
        var query = new GetCustomerByValueQuery { Type = CustomerSearchValues.Code, Value = "123456789" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetBySsnAsync(It.IsAny<string>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByCodeAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetCustomerByValueQuery_Incorrect_Value()
    {
        var mockUnitOfWork = GetMockUnitOfWork();
        var handler = new GetCustomerByValueQuery.GetCustomerByValueHandler(
            mockUnitOfWork.Object,
            new GetCustomerByValueQuery.GetCustomerByValueValidator()
        );
        var query = new GetCustomerByValueQuery { Type = "Invalid value" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsT2);
        Assert.NotEmpty(result.AsT2.Errors);
        Assert.Equal(2, result.AsT2.Errors.Count());
        Assert.Contains(result.AsT2.Errors, x => x.PropertyName == nameof(query.Type));
        Assert.Contains(result.AsT2.Errors, x => x.PropertyName == nameof(query.Value));
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never());
    }

    [Fact]
    public async Task GetCustomerByValueQuery_Not_Found()
    {
        var mockUnitOfWork = GetMockUnitOfWork();
        mockUnitOfWork.Setup(x => x.CustomerRepository.GetBySsnAsync(It.IsAny<string>()))
            .ReturnsAsync(null as Customer);

        var handler = new GetCustomerByValueQuery.GetCustomerByValueHandler(
            mockUnitOfWork.Object,
            new GetCustomerByValueQuery.GetCustomerByValueValidator()
        );
        var query = new GetCustomerByValueQuery { Type = CustomerSearchValues.Ssn, Value = "12345678-1234" };

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.True(result.IsT1);

        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetBySsnAsync(It.IsAny<string>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByCodeAsync(It.IsAny<string>()), Times.Never);
    }

    private static Mock<IUnitOfWork> GetMockUnitOfWork()
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var returnCustomer = new Customer();
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        mockCustomerRepository
            .Setup(x => x.GetByCodeAsync(It.IsAny<string>()))
            .ReturnsAsync(returnCustomer);
        mockCustomerRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(returnCustomer);
        mockCustomerRepository
            .Setup(x => x.GetBySsnAsync(It.IsAny<string>()))
            .ReturnsAsync(returnCustomer);

        mockUnitOfWork.SetupGet(x => x.CustomerRepository).Returns(mockCustomerRepository.Object);

        return mockUnitOfWork;
    }
}