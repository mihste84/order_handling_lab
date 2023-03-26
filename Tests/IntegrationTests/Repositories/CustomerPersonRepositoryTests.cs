using Microsoft.Data.SqlClient;

namespace Tests.IntegrationTests.Repositories;

public class CustomerPersonRepositoryTests : TestBase
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
    public CustomerPersonRepositoryTests(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    [Fact]
    public async Task Test_Insert_And_Select_CustomerPerson_Pass()
    {
        await ResetDatabaseAsync(_connectionString);
        var (id, personId) = await InsertCustomerPersonAsync();

        Assert.NotNull(personId);
        Assert.NotNull(id);

        var customerPerson = await _unitOfWork.CustomerPersonRepository.GetByIdAsync(personId.Value, CancellationToken.None);
        Assert.NotNull(customerPerson);
        Assert.Equal("John", customerPerson.FirstName);
        Assert.Equal("Doe", customerPerson.LastName);
        Assert.Equal(id, customerPerson.CustomerId);
    }

    [Fact]
    public async Task Test_Insert_Second_CustomerPerson_Fail()
    {
        await ResetDatabaseAsync(_connectionString);
        var (id, personId) = await InsertCustomerPersonAsync();

        Assert.NotNull(personId);
        Assert.NotNull(id);

        var duplicatePerson = new CustomerPerson {
            FirstName = "Jane",
            LastName = "Doe",
            CustomerId = id,
            CreatedBy = "Test",
            UpdatedBy = "Test",
            Ssn = "987-65-4321",
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        var ex = await Assert.ThrowsAsync<SqlException>(async () => await _unitOfWork.CustomerPersonRepository.InsertAsync(duplicatePerson, CancellationToken.None));

        Assert.NotNull(ex);
        Assert.IsType<SqlException>(ex);
        Assert.Contains("IX_CustomerPersons_CustomerId", ex.Message);   
    }

    private async Task<(int? CustomerId, int? CustomerPersonId)> InsertCustomerPersonAsync() {
        var customer = new Customer { 
            Active = true, 
            CreatedBy = "Test", 
            UpdatedBy = "Test", 
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var id = await _unitOfWork.CustomerRepository.InsertAsync(customer, CancellationToken.None);

        var customerPerson = new CustomerPerson
        {
            FirstName = "John",
            LastName = "Doe",
            CustomerId = id,
            CreatedBy = "Test",
            UpdatedBy = "Test",
            Ssn = "123-45-6789",
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        var personId = await _unitOfWork.CustomerPersonRepository.InsertAsync(customerPerson, CancellationToken.None);

        return (id, personId);  
    }
}