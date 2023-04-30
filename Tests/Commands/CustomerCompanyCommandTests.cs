using AppLogic.Common.Services;
using AppLogic.Customers.CustomerCompanies.Commands;

namespace Tests.Commands;

public class CustomerCompanyCommandTests
{
    [Fact]
    public async Task InsertCustomerCompanyCommand_Pass() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepoitoryReturnValues());
               
        var command = new InsertCustomerCompanyCommand {
            Name = "Test Company",
            Code = "TC",
            CustomerId = 1
        };
        var handler = new InsertCustomerCompanyCommand.InsertCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerCompanyCommand.InsertCustomerCompanyValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.InsertAsync(It.IsAny<CustomerCompany>()), Times.Once);
    }

    [Fact]
    public async Task InsertCustomerCompanyCommand_Invalid_Fail() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());
               
        var command = new InsertCustomerCompanyCommand {           
            CustomerId = 1
        };
        var handler = new InsertCustomerCompanyCommand.InsertCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerCompanyCommand.InsertCustomerCompanyValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Equal(2, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.InsertAsync(It.IsAny<CustomerCompany>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerCompanyCommand_No_Insert_Fail() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues() );
        mockUnitOfWork.Setup(x => x.CustomerCompanyRepository.InsertAsync(It.IsAny<CustomerCompany>())).ReturnsAsync((int?)null);

        var command = new InsertCustomerCompanyCommand {           
            Name = "Test Company",
            Code = "TC",
            CustomerId = 1
        };
        var handler = new InsertCustomerCompanyCommand.InsertCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerCompanyCommand.InsertCustomerCompanyValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to insert customer company.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.InsertAsync(It.IsAny<CustomerCompany>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateCustomerCompanyCommand_Pass() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerCompanyCommand {           
            Name = "Test Company",
            Code = "TC",
            CustomerCompanyId = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerCompanyCommand.UpdateCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerCompanyCommand.UpdateCustomerCompanyValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.UpdateAsync(It.IsAny<CustomerCompany>()), Times.Once);
    } 

    [Fact]
    public async Task UpdateCustomerCompanyCommand_Invalid_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerCompanyCommand {           
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerCompanyCommand.UpdateCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerCompanyCommand.UpdateCustomerCompanyValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT3);
        var errors = result.AsT3.Errors;
        Assert.Equal(3, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.UpdateAsync(It.IsAny<CustomerCompany>()), Times.Never);
    } 

    [Fact]
    public async Task UpdateCustomerCompanyCommand_Not_Found_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerCompanyRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CustomerCompany?)null);

        var command = new UpdateCustomerCompanyCommand {   
            Name = "Test Company",
            Code = "TC",
            CustomerCompanyId = 1,        
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerCompanyCommand.UpdateCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerCompanyCommand.UpdateCustomerCompanyValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.UpdateAsync(It.IsAny<CustomerCompany>()), Times.Never);
    } 

    [Fact]
    public async Task UpdateCustomerCompanyCommand_Wrong_RowVersion_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerCompanyCommand {   
            Name = "Test Company",
            Code = "TC",
            CustomerCompanyId = 1,        
            RowVersion = new byte[] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerCompanyCommand.UpdateCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerCompanyCommand.UpdateCustomerCompanyValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        Assert.Equal("Customer has been updated by another user. Please refresh the page and try again.", result.AsT2.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.UpdateAsync(It.IsAny<CustomerCompany>()), Times.Never);
    } 

    [Fact]
    public async Task DeleteCustomerCompanyCommand_Pass() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new DeleteCustomerCompanyCommand {           
            CustomerCompanyId = 1
        };
        var handler = new DeleteCustomerCompanyCommand.DeleteCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerCompanyCommand.DeleteCustomerCompanyValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    } 

    [Fact]
    public async Task DeleteCustomerCompanyCommand_Invalid_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new DeleteCustomerCompanyCommand {};
        var handler = new DeleteCustomerCompanyCommand.DeleteCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerCompanyCommand.DeleteCustomerCompanyValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.Never());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    } 

    [Fact]
    public async Task DeleteCustomerCompanyCommand_Error_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues() { DeleCustomer = false});

        var command = new DeleteCustomerCompanyCommand {          
            CustomerCompanyId = 2
        };
        var handler = new DeleteCustomerCompanyCommand.DeleteCustomerCompanyHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerCompanyCommand.DeleteCustomerCompanyValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to delete customer Company.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerCompanyRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerCompanyRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    } 

    private record RepoitoryReturnValues(
        int CustomerCompanyId = 1,
        int CustomerId = 1,
        bool CustomerContactInfo = true,
        bool CustomerAddresses = true,
        bool UpdateCustomer = true,
        bool DeleCustomer = true
    );

    private Mock<IUnitOfWork> GetMockUnitOfWork(RepoitoryReturnValues returnValues) {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        
        var mockCustomerCompanyRepository = new Mock<ICustomerCompanyRepository>();
        mockCustomerCompanyRepository
            .Setup(x => x.InsertAsync(It.IsAny<CustomerCompany>()))
            .ReturnsAsync(returnValues.CustomerCompanyId);
        mockCustomerCompanyRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnValues.DeleCustomer);
        mockCustomerCompanyRepository
            .Setup(x => x.UpdateAsync(It.IsAny<CustomerCompany>()))
            .ReturnsAsync(returnValues.UpdateCustomer);
        mockCustomerCompanyRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new CustomerCompany() { 
                Id = returnValues.CustomerId, 
                CustomerId = returnValues.CustomerId,
                Name = "Test Company",
                Code = "1234567890",
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } 
            });

        mockUnitOfWork.SetupGet(x => x.CustomerCompanyRepository).Returns(mockCustomerCompanyRepository.Object);

        return mockUnitOfWork;
    }
}