namespace Tests.IntegrationTests.Repositories;

public class CustomerRepositoryTests : TestBase
{
    public CustomerRepositoryTests(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    [Fact]
    public async Task Test_Select_Customer_By_Ssn_Pass()
    {
        await InsertCustomerPersonAsync();

        var customer = await _unitOfWork.CustomerRepository.GetBySsnAsync("123-45-6789");
        Assert.NotNull(customer);
        Assert.NotNull(customer.CustomerPerson);
        Assert.True(customer.Active);
        Assert.Equal("John", customer.CustomerPerson.FirstName);
        Assert.Equal("Doe", customer.CustomerPerson.LastName);
    }

    [Fact]
    public async Task Test_Select_Company_By_Code_Pass()
    {
        await InsertCustomerCompanyAsync();

        var company = await _unitOfWork.CustomerRepository.GetByCodeAsync("123");
        Assert.NotNull(company);
        Assert.NotNull(company.CustomerCompany);
        Assert.True(company.Active);
        Assert.Equal("123", company.CustomerCompany.Code);
        Assert.Equal("Test Company", company.CustomerCompany.Name);
    }

    private async Task<(int? CustomerId, int? CustomerPersonId)> InsertCustomerPersonAsync() {
        var customer = new Customer { 
            Active = true, 
            CreatedBy = "Test", 
            UpdatedBy = "Test", 
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var id = await _unitOfWork.CustomerRepository.InsertAsync(customer);

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
        var personId = await _unitOfWork.CustomerPersonRepository.InsertAsync(customerPerson);

        return (id, personId);  
    }

    private async Task<(int? CustomerId, int? CustomerCompanyId)> InsertCustomerCompanyAsync() {
        var customer = new Customer { 
            Active = true, 
            CreatedBy = "Test", 
            UpdatedBy = "Test", 
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var id = await _unitOfWork.CustomerRepository.InsertAsync(customer);

        var customerCompany = new CustomerCompany { 
            Code = "123",
            Name = "Test Company",
            CustomerId = id, 
            CreatedBy = "Test", 
            UpdatedBy = "Test",
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var CompanyId = await _unitOfWork.CustomerCompanyRepository.InsertAsync(customerCompany);

        return (id, CompanyId);  
    }
}