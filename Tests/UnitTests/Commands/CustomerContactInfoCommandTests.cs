using Common.Services;
using Customers.CustomerContactInfos.Commands;

namespace UnitTests.Commands;

public class CustomerContactInfoCommandTests
{
    [Fact]
    public async Task InsertCustomerContactInfoCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1
        };
        var handler = new InsertCustomerContactInfoCommand.InsertCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerContactInfoCommand.InsertCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.InsertAsync(It.IsAny<CustomerContactInfo>()), Times.Once);
    }

    [Fact]
    public async Task InsertCustomerContactInfoCommand_IsPrimary()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1
        };
        var handler = new InsertCustomerContactInfoCommand.InsertCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerContactInfoCommand.InsertCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.InsertAsync(It.IsAny<CustomerContactInfo>()), Times.Once);
    }

    [Fact]
    public async Task InsertCustomerContactInfoCommand_Invalid()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new InsertCustomerContactInfoCommand
        {
            CustomerId = 1
        };
        var handler = new InsertCustomerContactInfoCommand.InsertCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerContactInfoCommand.InsertCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Equal(3, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.InsertAsync(It.IsAny<CustomerContactInfo>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerContactInfoCommand_No_Insert()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerContactInfoRepository.InsertAsync(It.IsAny<CustomerContactInfo>())).ReturnsAsync((SqlResult?)null);

        var command = new InsertCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            CustomerId = 1
        };
        var handler = new InsertCustomerContactInfoCommand.InsertCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new InsertCustomerContactInfoCommand.InsertCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to insert customer contact info.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.InsertAsync(It.IsAny<CustomerContactInfo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerContactInfoCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.UpdateAsync(It.IsAny<CustomerContactInfo>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerContactInfoCommand_Invalid()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerContactInfoCommand
        {
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT3);
        var errors = result.AsT3.Errors;
        Assert.Equal(4, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.GetByIdAsync(It.IsAny<int>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.UpdateAsync(It.IsAny<CustomerContactInfo>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerContactInfoCommand_Not_Found()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerContactInfoRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CustomerContactInfo?)null);

        var command = new UpdateCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.UpdateAsync(It.IsAny<CustomerContactInfo>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerContactInfoCommand_Wrong_RowVersion()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new UpdateCustomerContactInfoCommand
        {
            Value = "+46704512343",
            Type = ContactInfoType.Phone,
            Id = 1,
            RowVersion = new byte[] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new TestAuthenticationService(),
            new UpdateCustomerContactInfoCommand.UpdateCustomerContactInfoValidator(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        Assert.Equal("Customer contact info has been updated by another user. Please refresh the page and try again.", result.AsT2.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.UpdateAsync(It.IsAny<CustomerContactInfo>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerContactInfoCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new DeleteCustomerContactInfoCommand
        {
            Id = 1
        };
        var handler = new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerContactInfoCommand_Invalid()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues());

        var command = new DeleteCustomerContactInfoCommand();
        var handler = new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Never());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerContactInfoCommand_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepositoryReturnValues() { Delete = false });

        var command = new DeleteCustomerContactInfoCommand
        {
            Id = 2
        };
        var handler = new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerContactInfoCommand.DeleteCustomerContactInfoValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to delete customer contact info.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerContactInfoRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    }

    private record RepositoryReturnValues(
        int CustomerContactInfoId = 1,
        int CustomerId = 1,
        bool CustomerContactInfo = true,
        bool Update = true,
        bool Delete = true
    );

    private static Mock<IUnitOfWork> GetMockUnitOfWork(RepositoryReturnValues returnValues)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var res = new SqlResult { Id = returnValues.CustomerContactInfoId };
        var mockCustomerContactInfoRepository = new Mock<ICustomerContactInfoRepository>();
        mockCustomerContactInfoRepository
            .Setup(x => x.InsertAsync(It.IsAny<CustomerContactInfo>()))
            .ReturnsAsync(res);
        mockCustomerContactInfoRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnValues.Delete);
        mockCustomerContactInfoRepository
            .Setup(x => x.UpdateAsync(It.IsAny<CustomerContactInfo>()))
            .ReturnsAsync(res);
        mockCustomerContactInfoRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new CustomerContactInfo()
            {
                Id = returnValues.CustomerContactInfoId,
                CustomerId = returnValues.CustomerId,
                Type = ContactInfoType.Email,
                Value = "stemih11@gmail.com",
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
            });

        mockUnitOfWork.SetupGet(x => x.CustomerContactInfoRepository).Returns(mockCustomerContactInfoRepository.Object);

        return mockUnitOfWork;
    }
}