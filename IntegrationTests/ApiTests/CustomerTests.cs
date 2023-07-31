using Customers.BaseCustomers.Commands;
using Customers.CommonModels;

namespace IntegrationTests.ApiTests;

[Order(2)]
public sealed class CustomerTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CustomerTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Insert_Customer_Person()
    {
        await _testBase.ResetDbAsync();
        var command = GetBaseInsertCustomerPersonCommand();

        var response = await InsertCustomerAsync(command);

        Assert.NotNull(response);
        Assert.NotNull(response.Id);

        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(response.Id.GetValueOrDefault(), true);
        Assert.NotNull(customer);
        Assert.NotNull(customer.CustomerPerson);
        Assert.Null(customer.CustomerCompany);
        Assert.Equal(command.Ssn, customer.CustomerPerson.Ssn);
        Assert.Equal(command.FirstName, customer.CustomerPerson.FirstName);
        Assert.Equal(command.LastName, customer.CustomerPerson.LastName);
        Assert.NotNull(customer.CustomerAddresses);
        Assert.NotEmpty(customer.CustomerAddresses);
        var address = command.CustomerAddresses!.First();
        Assert.Equal(address.Address, customer.CustomerAddresses.First().Address);
        Assert.Equal(address.CityId, customer.CustomerAddresses.First().CityId);
        Assert.Equal(address.CountryId, customer.CustomerAddresses.First().CountryId);
        Assert.Equal(address.IsPrimary, customer.CustomerAddresses.First().IsPrimary);
        Assert.Equal(address.PostArea, customer.CustomerAddresses.First().PostArea);
        Assert.NotNull(customer.CustomerContactInfos);
        Assert.NotEmpty(customer.CustomerContactInfos);
        var contactInfo = command.ContactInfo!.First();
        Assert.Equal(contactInfo.Type, customer.CustomerContactInfos.First().Type);
        Assert.Equal(contactInfo.Value, customer.CustomerContactInfos.First().Value);
    }

    [Fact]
    public async Task Insert_Customer_Company()
    {
        await _testBase.ResetDbAsync();
        var command = GetBaseInsertCustomerCompanyCommand();
        var response = await InsertCustomerAsync(command);

        Assert.NotNull(response);
        Assert.NotNull(response.Id);

        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(response.Id.GetValueOrDefault(), true);
        Assert.NotNull(customer);
        Assert.NotNull(customer.CustomerCompany);
        Assert.Null(customer.CustomerPerson);
        Assert.Equal(command.Name, customer.CustomerCompany.Name);
        Assert.Equal(command.Code, customer.CustomerCompany.Code);
        Assert.NotNull(customer.CustomerAddresses);
        Assert.NotEmpty(customer.CustomerAddresses);
        var address = command.CustomerAddresses!.First();
        Assert.Equal(address.Address, customer.CustomerAddresses.First().Address);
        Assert.Equal(address.CityId, customer.CustomerAddresses.First().CityId);
        Assert.Equal(address.CountryId, customer.CustomerAddresses.First().CountryId);
        Assert.Equal(address.IsPrimary, customer.CustomerAddresses.First().IsPrimary);
        Assert.Equal(address.PostArea, customer.CustomerAddresses.First().PostArea);
        Assert.NotNull(customer.CustomerContactInfos);
        Assert.NotEmpty(customer.CustomerContactInfos);
        var contactInfo = command.ContactInfo!.First();
        Assert.Equal(contactInfo.Type, customer.CustomerContactInfos.First().Type);
        Assert.Equal(contactInfo.Value, customer.CustomerContactInfos.First().Value);
    }

    [Fact]
    public async Task Update_Customer_Person()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerPersonCommand();

        var insertResponse = await InsertCustomerAsync(insertCommand);

        var updateCommand = new UpdateCustomerCommand
        {
            Id = insertResponse!.Id,
            IsActive = false,
            FirstName = "Jane",
            LastName = "Doe",
            Ssn = "87654321-1234",
            MiddleName = "M",
            RowVersion = insertResponse.RowVersion
        };

        var updateResponse = await UpdateCustomerAsync(updateCommand);
        Assert.NotNull(updateResponse);
        Assert.NotNull(updateResponse.Id);
        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(updateCommand.Id.GetValueOrDefault(), false);
        Assert.NotNull(customer);
        Assert.NotNull(customer.CustomerPerson);
        Assert.Null(customer.CustomerCompany);
        Assert.Equal(updateCommand.IsActive, customer.Active);
        Assert.Equal(updateCommand.Ssn, customer.CustomerPerson.Ssn);
        Assert.Equal(updateCommand.FirstName, customer.CustomerPerson.FirstName);
        Assert.Equal(updateCommand.LastName, customer.CustomerPerson.LastName);
        Assert.Equal(updateCommand.MiddleName, customer.CustomerPerson.MiddleName);
        Assert.Equal(updateResponse.RowVersion, customer.RowVersion);
        Assert.Equal(updateResponse.Id, customer.Id);
    }

    [Fact]
    public async Task Update_Customer_Company()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerCompanyCommand();

        var insertResponse = await InsertCustomerAsync(insertCommand);

        var updateCommand = new UpdateCustomerCommand
        {
            Id = insertResponse!.Id,
            IsCompany = true,
            IsActive = false,
            Name = "Test comp",
            Code = "987654321",
            RowVersion = insertResponse.RowVersion
        };

        var updateResponse = await UpdateCustomerAsync(updateCommand);
        Assert.NotNull(updateResponse);
        Assert.NotNull(updateResponse.Id);
        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(updateCommand.Id.GetValueOrDefault(), false);
        Assert.NotNull(customer);
        Assert.NotNull(customer.CustomerCompany);
        Assert.Null(customer.CustomerPerson);
        Assert.Equal(updateCommand.IsActive, customer.Active);
        Assert.Equal(updateCommand.Name, customer.CustomerCompany.Name);
        Assert.Equal(updateCommand.Code, customer.CustomerCompany.Code);
        Assert.Equal(updateResponse.RowVersion, customer.RowVersion);
        Assert.Equal(updateResponse.Id, customer.Id);
    }

    [Fact]
    public async Task Delete_Customer_Person()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerPersonCommand();

        var insertResponse = await InsertCustomerAsync(insertCommand);

        var deleteResponse = await _testBase.HttpClient.DeleteAsync("/api/customer/" + insertResponse!.Id);
        Assert.True(deleteResponse.IsSuccessStatusCode);
        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(insertResponse.Id.GetValueOrDefault(), false);
        Assert.Null(customer);
    }

    [Fact]
    public async Task Delete_Customer_Company()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerCompanyCommand();

        var insertResponse = await InsertCustomerAsync(insertCommand);

        var deleteResponse = await _testBase.HttpClient.DeleteAsync("/api/customer/" + insertResponse!.Id);
        Assert.True(deleteResponse.IsSuccessStatusCode);
        var customer = await _testBase.UnitOfWork.CustomerRepository.GetByIdAsync(insertResponse.Id.GetValueOrDefault(), false);
        Assert.Null(customer);
    }

    private async Task<SqlResult?> InsertCustomerAsync(InsertCustomerCommand model)
    {
        var res = await _testBase.HttpClient.PostAsJsonAsync("/api/customer", model);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<SqlResult>();
    }

    private async Task<SqlResult?> UpdateCustomerAsync(UpdateCustomerCommand model)
    {
        var res = await _testBase.HttpClient.PutAsJsonAsync("/api/customer", model);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadFromJsonAsync<SqlResult>();
    }

    private static InsertCustomerCommand GetBaseInsertCustomerCompanyCommand()
    => new()
    {
        Name = "John",
        Code = "123456789",
        IsCompany = true,
        CustomerAddresses = new[] {
            new CustomerAddressModel {
                Address = "123 Main St",
                CityId = 1,
                CountryId = 1,
                IsPrimary = true,
                PostArea = "Stockholm",
                ZipCode = "12345"
            }
        },
        ContactInfo = new[] {
            new CustomerContactInfoModel {
                Type = ContactInfoType.Email,
                Value = "test@mail.com"
            }
        }
    };

    private static InsertCustomerCommand GetBaseInsertCustomerPersonCommand()
    => new()
    {
        Ssn = "12345678-1234",
        FirstName = "John",
        LastName = "Doe",
        IsCompany = false,
        CustomerAddresses = new[] {
            new CustomerAddressModel {
                Address = "123 Main St",
                CityId = 1,
                CountryId = 1,
                IsPrimary = true,
                PostArea = "Stockholm",
                ZipCode = "12345"
            }
        },
        ContactInfo = new[] {
            new CustomerContactInfoModel {
                Type = ContactInfoType.Email,
                Value = "test@mail.com"
            }
        }
    };
}