using Customers.BaseCustomers.Commands;
using Customers.CommonModels;
using Customers.CustomerContactInfos.Commands;

namespace IntegrationTests.ApiTests;

[Order(2)]
public sealed class CustomerContactInfoTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CustomerContactInfoTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Insert_New_Contact_Info()
    {
        await _testBase.ResetDbAsync();
        var result = await _testBase.InsertCustomerAsync();
        var command = new InsertCustomerContactInfoCommand
        {
            CustomerId = result!.Id,
            Type = ContactInfoType.Email,
            Value = "stemih11@gmail.com"
        };

        var response = await _testBase.HttpClient.PostAsJsonAsync("/api/customercontactinfo", command);
        response.EnsureSuccessStatusCode();

        var infoResult = await response.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(infoResult);
        var customerContactInfos = await _testBase.UnitOfWork.CustomerContactInfoRepository.GetByCustomerIdAsync(result.Id!.Value);
        Assert.NotNull(customerContactInfos);
        Assert.Equal(2, customerContactInfos.Count());
        var last = customerContactInfos.Last();
        Assert.Equal(command.Type, last.Type);
        Assert.Equal(command.Value, last.Value);
        Assert.Equal(command.CustomerId, last.CustomerId);
    }

    [Fact]
    public async Task Update_Contact_Info()
    {
        await _testBase.ResetDbAsync();
        var result = await _testBase.InsertCustomerAsync();
        var customerContactInfos = await _testBase.UnitOfWork.CustomerContactInfoRepository.GetByCustomerIdAsync(result!.Id!.Value);
        var command = new UpdateCustomerContactInfoCommand
        {
            Id = customerContactInfos!.First().Id,
            Type = ContactInfoType.Phone,
            Value = "+467065465445",
            RowVersion = customerContactInfos!.First().RowVersion
        };

        var response = await _testBase.HttpClient.PutAsJsonAsync("/api/customercontactinfo", command);
        response.EnsureSuccessStatusCode();

        var infoResult = await response.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(infoResult);
        customerContactInfos = await _testBase.UnitOfWork.CustomerContactInfoRepository.GetByCustomerIdAsync(result.Id!.Value);
        Assert.NotNull(customerContactInfos);
        Assert.Single(customerContactInfos);
        var first = customerContactInfos.First();
        Assert.Equal(command.Type, first.Type);
        Assert.Equal(command.Value, first.Value);
        Assert.Equal(infoResult.RowVersion, first.RowVersion);
        Assert.Equal(infoResult.Id, first.Id);
    }

    [Fact]
    public async Task Delete_Contact_Info()
    {
        await _testBase.ResetDbAsync();
        var command = new InsertCustomerCommand()
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
                },
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Phone,
                    Value = "+467065465445"
                }
            }
        };
        var result = await _testBase.InsertCustomerAsync(command);
        var customerContactInfos = await _testBase.UnitOfWork.CustomerContactInfoRepository.GetByCustomerIdAsync(result!.Id!.Value);

        var response = await _testBase.HttpClient.DeleteAsync("/api/customercontactinfo/" + customerContactInfos!.First().Id);
        response.EnsureSuccessStatusCode();

        customerContactInfos = await _testBase.UnitOfWork.CustomerContactInfoRepository.GetByCustomerIdAsync(result.Id!.Value);
        Assert.NotNull(customerContactInfos);
        Assert.Single(customerContactInfos);
        var first = customerContactInfos.First();
        Assert.Equal(ContactInfoType.Phone, first.Type);
        Assert.Equal("+467065465445", first.Value);
    }
}