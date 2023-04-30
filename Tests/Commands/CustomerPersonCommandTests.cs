using AppLogic.Common.Services;
using AppLogic.Customers.CustomerPersons.Commands;

namespace Tests.Commands;

public class CustomerPersonCommandTests
{
    [Fact]
    public async Task InsertCustomerPersonCommand_Pass() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepoitoryReturnValues());
               
        var command = new InsertCustomerPersonCommand {
            FirstName = "John",
            LastName = "Doe",
            Ssn = "1234567890",
            MiddleName = "M",
            CustomerId = 1
        };
        var handler = new InsertCustomerPersonCommand.InsertCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerPersonCommand.InsertCustomerPersonValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.InsertAsync(It.IsAny<CustomerPerson>()), Times.Once);
    }

    [Fact]
    public async Task InsertCustomerPersonCommand_Invalid_Fail() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());
               
        var command = new InsertCustomerPersonCommand {           
            MiddleName = "M"
        };
        var handler = new InsertCustomerPersonCommand.InsertCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerPersonCommand.InsertCustomerPersonValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Equal(4, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.InsertAsync(It.IsAny<CustomerPerson>()), Times.Never);
    }

    [Fact]
    public async Task InsertCustomerPersonCommand_No_Insert_Fail() 
    {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerPersonRepository.InsertAsync(It.IsAny<CustomerPerson>())).ReturnsAsync((int?)null);

        var command = new InsertCustomerPersonCommand {           
            FirstName = "John",
            LastName = "Doe",
            Ssn = "1234567890",
            MiddleName = "M",
            CustomerId = 1
        };
        var handler = new InsertCustomerPersonCommand.InsertCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new InsertCustomerPersonCommand.InsertCustomerPersonValidator(), 
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to insert customer person.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.InsertAsync(It.IsAny<CustomerPerson>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateCustomerPersonCommand_Pass() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerPersonCommand {           
            FirstName = "Stefan",
            LastName = "Mihailovic",
            Ssn = "1234567890",
            CustomerPersonId = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerPersonCommand.UpdateCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerPersonCommand.UpdateCustomerPersonValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.UpdateAsync(It.IsAny<CustomerPerson>()), Times.Once);
    } 

    [Fact]
    public async Task UpdateCustomerPersonCommand_Invalid_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerPersonCommand {           
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerPersonCommand.UpdateCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerPersonCommand.UpdateCustomerPersonValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT3);
        var errors = result.AsT3.Errors;
        Assert.Equal(4, errors.Count());
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.GetByIdAsync(It.IsAny<int>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.UpdateAsync(It.IsAny<CustomerPerson>()), Times.Never);
    } 

    [Fact]
    public async Task UpdateCustomerPersonCommand_Not_Found_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerPersonRepository.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((CustomerPerson?)null);

        var command = new UpdateCustomerPersonCommand {   
            FirstName = "Stefan",
            LastName = "Mihailovic",
            Ssn = "1234567890",
            CustomerPersonId = 1,        
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerPersonCommand.UpdateCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerPersonCommand.UpdateCustomerPersonValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.UpdateAsync(It.IsAny<CustomerPerson>()), Times.Never);
    } 

    [Fact]
    public async Task UpdateCustomerPersonCommand_Wrong_RowVersion_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new UpdateCustomerPersonCommand {   
            FirstName = "Stefan",
            LastName = "Mihailovic",
            Ssn = "1234567890",
            CustomerPersonId = 1,        
            RowVersion = new byte[] { 11, 2, 3, 4, 5, 6, 7, 8, 9, 10 }
        };
        var handler = new UpdateCustomerPersonCommand.UpdateCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new UpdateCustomerPersonCommand.UpdateCustomerPersonValidator(), 
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        Assert.Equal("Customer has been updated by another user. Please refresh the page and try again.", result.AsT2.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce);
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.GetByIdAsync(It.IsAny<int>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.UpdateAsync(It.IsAny<CustomerPerson>()), Times.Never);
    } 

    [Fact]
    public async Task DeleteCustomerPersonCommand_Pass() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new DeleteCustomerPersonCommand {           
            CustomerPersonId = 1
        };
        var handler = new DeleteCustomerPersonCommand.DeleteCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerPersonCommand.DeleteCustomerPersonValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    } 

    [Fact]
    public async Task DeleteCustomerPersonCommand_Invalid_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues());

        var command = new DeleteCustomerPersonCommand {};
        var handler = new DeleteCustomerPersonCommand.DeleteCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerPersonCommand.DeleteCustomerPersonValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var errors = result.AsT2.Errors;
        Assert.Single(errors);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.Never());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Never);
    } 

    [Fact]
    public async Task DeleteCustomerPersonCommand_Error_Fail() {
        var mockUnitOfWork = GetMockUnitOfWork(returnValues: new RepoitoryReturnValues() { DeleCustomer = false});

        var command = new DeleteCustomerPersonCommand {          
            CustomerPersonId = 2
        };
        var handler = new DeleteCustomerPersonCommand.DeleteCustomerPersonHandler(
            mockUnitOfWork.Object, 
            new DeleteCustomerPersonCommand.DeleteCustomerPersonValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        Assert.Equal("Failed to delete customer person.", result.AsT1.Value);
        mockUnitOfWork.VerifyGet(x => x.CustomerPersonRepository, Times.AtLeastOnce());
        mockUnitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerPersonRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    } 

    private record RepoitoryReturnValues(
        int CustomerPersonId = 1,
        int CustomerId = 1,
        bool CustomerContactInfo = true,
        bool CustomerAddresses = true,
        bool UpdateCustomer = true,
        bool DeleCustomer = true
    );

    private Mock<IUnitOfWork> GetMockUnitOfWork(RepoitoryReturnValues returnValues) {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        
        var mockCustomerPersonRepository = new Mock<ICustomerPersonRepository>();
        mockCustomerPersonRepository
            .Setup(x => x.InsertAsync(It.IsAny<CustomerPerson>()))
            .ReturnsAsync(returnValues.CustomerPersonId);
        mockCustomerPersonRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnValues.DeleCustomer);
        mockCustomerPersonRepository
            .Setup(x => x.UpdateAsync(It.IsAny<CustomerPerson>()))
            .ReturnsAsync(returnValues.UpdateCustomer);
        mockCustomerPersonRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new CustomerPerson() { 
                Id = returnValues.CustomerId, 
                CustomerId = returnValues.CustomerId,
                FirstName = "John",
                LastName = "Doe",
                MiddleName = "M",
                Ssn = "1234567890",
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } 
            });

        mockUnitOfWork.SetupGet(x => x.CustomerPersonRepository).Returns(mockCustomerPersonRepository.Object);

        return mockUnitOfWork;
    }
}