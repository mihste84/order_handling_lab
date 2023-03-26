using Microsoft.Data.SqlClient;

namespace Tests.IntegrationTests.Repositories;

public class CustomerCompanyRepositoryTests : TestBase
{
    private readonly string _connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")!;
    public CustomerCompanyRepositoryTests(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    [Fact]
    public async Task Test_Insert_And_Select_CustomerCompany_Pass()
    {
        await ResetDatabaseAsync(_connectionString);
        var (id, CompanyId) = await InsertCustomerCompanyAsync();

        Assert.NotNull(CompanyId);
        Assert.NotNull(id);

        var customerCompany = await _unitOfWork.CustomerCompanyRepository.GetByIdAsync(CompanyId.Value, CancellationToken.None);
        Assert.NotNull(customerCompany);
        Assert.Equal("123", customerCompany.Code);
        Assert.Equal("Test Company", customerCompany.Name);
        Assert.Equal(id, customerCompany.CustomerId);
    }

    [Fact]
    public async Task Test_Insert_Second_CustomerCompany_Fail()
    {
        await ResetDatabaseAsync(_connectionString);
        var (id, CompanyId) = await InsertCustomerCompanyAsync();

        Assert.NotNull(CompanyId);
        Assert.NotNull(id);

        var duplicateCompany = new CustomerCompany {
            Code = "987",
            Name = "Test Company duplicate",
            CustomerId = id,
            CreatedBy = "Test",
            UpdatedBy = "Test",
            Created = DateTime.Now,
            Updated = DateTime.Now
        };
        var ex = await Assert.ThrowsAsync<SqlException>(async () => await _unitOfWork.CustomerCompanyRepository.InsertAsync(duplicateCompany, CancellationToken.None));

        Assert.NotNull(ex);
        Assert.IsType<SqlException>(ex);
        Assert.Contains("IX_CustomerCompanies_CustomerId", ex.Message);   
    }

    private async Task<(int? CustomerId, int? CustomerCompanyId)> InsertCustomerCompanyAsync() {
        var customer = new Customer { 
            Active = true, 
            CreatedBy = "Test", 
            UpdatedBy = "Test", 
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var id = await _unitOfWork.CustomerRepository.InsertAsync(customer, CancellationToken.None);

        var customerCompany = new CustomerCompany { 
            Code = "123",
            Name = "Test Company",
            CustomerId = id, 
            CreatedBy = "Test", 
            UpdatedBy = "Test", 
            Created = DateTime.Now, 
            Updated = DateTime.Now 
        };
        var CompanyId = await _unitOfWork.CustomerCompanyRepository.InsertAsync(customerCompany, CancellationToken.None);

        return (id, CompanyId);  
    }
}