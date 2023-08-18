using Common.Services;
using Customers.CustomerAddresses.Commands;

namespace UnitTests.Commands;

public class CustomerAddressCommandTests
{
    [Fact]
    public async Task InsertCustomerAddressCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = false,
            PostArea = "Stockholm",
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1,
            ZipCode = "1234"
        };
        var handler = new InsertCustomerAddressCommand.InsertCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerAddressCommand.InsertCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.InsertAsync(It.IsAny<CustomerAddress>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerAddressCommand_IsPrimary()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Stockholm",
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1,
            ZipCode = "1234"
        };
        var handler = new InsertCustomerAddressCommand.InsertCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerAddressCommand.InsertCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.InsertAsync(It.IsAny<CustomerAddress>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Once);
    }

    [Fact]
    public async Task InsertCustomerAddressCommand_Validation_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new InsertCustomerAddressCommand
        {
            CustomerId = 1
        };
        var handler = new InsertCustomerAddressCommand.InsertCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerAddressCommand.InsertCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Equal(6, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.InsertAsync(It.IsAny<CustomerAddress>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerAddressCommand_Address_Count_Validation_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork
            .Setup(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()))
            .ReturnsAsync(3);
        var command = new InsertCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Stockholm",
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1,
            ZipCode = "1234"
        };
        var handler = new InsertCustomerAddressCommand.InsertCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerAddressCommand.InsertCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        Assert.Contains("Address count", errors.Select(x => x.PropertyName));
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()), Times.Once());
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerAddressCommand_No_Insert()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerAddressRepository.InsertAsync(It.IsAny<CustomerAddress>())).ReturnsAsync((SqlResult?)null);

        var command = new InsertCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = false,
            PostArea = "Stockholm",
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1,
            ZipCode = "1234"
        };
        var handler = new InsertCustomerAddressCommand.InsertCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerAddressCommand.InsertCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to insert customer address.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.InsertAsync(It.IsAny<CustomerAddress>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAddressCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = false,
            PostArea = "Stockholm",
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            ZipCode = "1234",
            CustomerId = 1
        };
        var handler = new UpdateCustomerAddressCommand.UpdateCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerAddressCommand.UpdateCustomerAddressValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.UpdateAsync(It.IsAny<CustomerAddress>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAddressCommand_IsPrimary()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Stockholm",
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            ZipCode = "1234",
            CustomerId = 1
        };
        var handler = new UpdateCustomerAddressCommand.UpdateCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerAddressCommand.UpdateCustomerAddressValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.UpdateAsync(It.IsAny<CustomerAddress>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerAddressCommand_Invalid()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerAddressCommand
        {
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerAddressCommand.UpdateCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerAddressCommand.UpdateCustomerAddressValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT3);
        var errors = result.AsT3.Errors;
        Assert.Equal(8, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.UpdateAsync(It.IsAny<CustomerAddress>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAddressCommand_Not_Found()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CustomerAddress?)null);

        var command = new UpdateCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Stockholm",
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            ZipCode = "1234",
            CustomerId = 2
        };
        var handler = new UpdateCustomerAddressCommand.UpdateCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerAddressCommand.UpdateCustomerAddressValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.UpdateAsync(It.IsAny<CustomerAddress>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerAddressCommand_Wrong_RowVersion()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerAddressCommand
        {
            Address = "Test Address",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Stockholm",
            Id = 1,
            RowVersion = new byte[] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            ZipCode = "1234",
            CustomerId = 1
        };
        var handler = new UpdateCustomerAddressCommand.UpdateCustomerAddressHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerAddressCommand.UpdateCustomerAddressValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Customer address has been updated by another user. Please refresh the page and try again.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.UpdateAsync(It.IsAny<CustomerAddress>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.RemoveAllPrimaryAsync(It.IsAny<int?>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerAddressCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork
            .Setup(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()))
            .ReturnsAsync(2);
        var command = new DeleteCustomerAddressCommand
        {
            Id = 1
        };
        var handler = new DeleteCustomerAddressCommand.DeleteCustomerAddressHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerAddressCommand.DeleteCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerAddressCommand_Validation_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork
            .Setup(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()))
            .ReturnsAsync(2);
        var command = new DeleteCustomerAddressCommand();
        var handler = new DeleteCustomerAddressCommand.DeleteCustomerAddressHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerAddressCommand.DeleteCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Never());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerAddressCommand_Address_Count_Validation_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new DeleteCustomerAddressCommand
        {
            Id = 1
        };
        var handler = new DeleteCustomerAddressCommand.DeleteCustomerAddressHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerAddressCommand.DeleteCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        Assert.Contains("Address count", errors.Select(x => x.PropertyName));
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Exactly(2));
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()), Times.Once());
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.GetByIdAsync(It.IsAny<int>()), Times.Once());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerAddressCommand_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues() { Delete = false });
        mockUnitOfWork
            .Setup(x => x.CustomerAddressRepository.GetCountByCustomerIdAsync(It.IsAny<int>()))
            .ReturnsAsync(2);
        var command = new DeleteCustomerAddressCommand
        {
            Id = 2
        };
        var handler = new DeleteCustomerAddressCommand.DeleteCustomerAddressHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerAddressCommand.DeleteCustomerAddressValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to delete customer address.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerAddressRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    }

    private record RepositoryReturnValues(
        int CustomerAddressId = 1,
        int CustomerId = 1,
        bool CustomerContactInfo = true,
        bool CustomerAddresses = true,
        bool Update = true,
        bool Delete = true
    );

    private static Mock<IUnitOfWork> GetMockUnitOfWork(RepositoryReturnValues returnValues)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();

        var mockCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
        mockCustomerAddressRepository
            .Setup(x => x.InsertAsync(It.IsAny<CustomerAddress>()))
            .ReturnsAsync(new SqlResult { Id = returnValues.CustomerAddressId });
        mockCustomerAddressRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnValues.Delete);
        mockCustomerAddressRepository
            .Setup(x => x.UpdateAsync(It.IsAny<CustomerAddress>()))
            .ReturnsAsync(new SqlResult { Id = returnValues.CustomerAddressId });
        mockCustomerAddressRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new CustomerAddress()
            {
                Id = returnValues.CustomerAddressId,
                CustomerId = returnValues.CustomerId,
                Address = "Test Address",
                ZipCode = "1234567890",
                CityId = 1,
                CountryId = 1,
                IsPrimary = true,
                PostArea = "Test Post Area",
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            });
        mockCustomerAddressRepository
            .Setup(x => x.GetCountByCustomerIdAsync(It.IsAny<int>()))
            .ReturnsAsync(1);

        mockUnitOfWork.SetupGet(x => x.CustomerAddressRepository).Returns(mockCustomerAddressRepository.Object);

        return mockUnitOfWork;
    }
}