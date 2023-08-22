using System.Net;
using Customers.BaseCustomers.Commands;
using Customers.CommonModels;
using Customers.CustomerAddresses.Commands;

namespace IntegrationTests.ApiTests;

[Order(1)]
public sealed class CustomerAddressTests : IClassFixture<TestBase>
{
    private readonly TestBase _testBase;

    public CustomerAddressTests(TestBase testBase)
    {
        _testBase = testBase;
    }

    [Fact]
    public async Task Insert_New_Address()
    {
        await _testBase.ResetDbAsync();
        var result = await _testBase.InsertCustomerAsync();
        var addressCommand = new InsertCustomerAddressCommand
        {
            CustomerId = result!.Id,
            Address = "931 Main St",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Solna",
            ZipCode = "98743"
        };

        var response = await _testBase.HttpClient.PostAsJsonAsync("/api/customeraddress", addressCommand);
        response.EnsureSuccessStatusCode();

        var addressResult = await response.Content.ReadFromJsonAsync<SqlResult>();
        Assert.NotNull(addressResult);
        var addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        Assert.NotNull(addresses);
        Assert.Equal(2, addresses.Count());
        Assert.Equal(false, addresses.First().IsPrimary);
        var newAddress = addresses.Last();
        Assert.NotNull(newAddress);
        Assert.True(newAddress!.IsPrimary);
        Assert.Equal(addressResult.Id, newAddress.Id);
        Assert.Equal(addressResult.RowVersion, newAddress.RowVersion);
        Assert.Equal(addressCommand.CustomerId, newAddress.CustomerId);
        Assert.Equal(addressCommand.Address, newAddress.Address);
        Assert.Equal(addressCommand.CityId, newAddress.CityId);
        Assert.Equal(addressCommand.CountryId, newAddress.CountryId);
        Assert.Equal(addressCommand.PostArea, newAddress.PostArea);
        Assert.Equal(addressCommand.ZipCode, newAddress.ZipCode);
    }

    [Fact]
    public async Task Insert_New_Address_Validation_Error_Too_Many()
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
                },
                new CustomerAddressModel {
                    Address = "987 Main St",
                    CityId = 1,
                    CountryId = 1,
                    IsPrimary = false,
                    PostArea = "Stockholm",
                    ZipCode = "54321"
                },
                new CustomerAddressModel {
                    Address = "777 Main St",
                    CityId = 1,
                    CountryId = 1,
                    IsPrimary = false,
                    PostArea = "Stockholm",
                    ZipCode = "54612"
                }
            },
            ContactInfo = new[] {
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Email,
                    Value = "test@mail.com"
                }
            }
        };
        var result = await _testBase.InsertCustomerAsync(command);
        var addressCommand = new InsertCustomerAddressCommand
        {
            CustomerId = result!.Id,
            Address = "931 Main St",
            CityId = 1,
            CountryId = 1,
            IsPrimary = false,
            PostArea = "Solna",
            ZipCode = "98743"
        };

        var response = await _testBase.HttpClient.PostAsJsonAsync("/api/customeraddress", addressCommand);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_Address()
    {
        await _testBase.ResetDbAsync();
        var result = await _testBase.InsertCustomerAsync();
        var addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        var address = addresses!.First();
        var updateCommand = new UpdateCustomerAddressCommand
        {
            Id = address.Id,
            CustomerId = address.CustomerId,
            Address = "931 Main St",
            CityId = 1,
            CountryId = 1,
            IsPrimary = true,
            PostArea = "Solna",
            ZipCode = "98743",
            RowVersion = address.RowVersion
        };

        var response = await _testBase.HttpClient.PutAsJsonAsync("/api/customeraddress", updateCommand);
        response.EnsureSuccessStatusCode();

        addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        Assert.NotNull(addresses);
        Assert.Single(addresses);
        address = addresses.First();
        Assert.NotNull(address);
        Assert.True(address!.IsPrimary);
        Assert.Equal(updateCommand.Id, address.Id);
        Assert.Equal(updateCommand.CustomerId, address.CustomerId);
        Assert.Equal(updateCommand.Address, address.Address);
        Assert.Equal(updateCommand.CityId, address.CityId);
        Assert.Equal(updateCommand.CountryId, address.CountryId);
        Assert.Equal(updateCommand.PostArea, address.PostArea);
        Assert.Equal(updateCommand.ZipCode, address.ZipCode);
    }

    [Fact]
    public async Task Delete_Address()
    {
        await _testBase.ResetDbAsync();
        var customerCommand = new InsertCustomerCommand()
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
                },
                new CustomerAddressModel {
                    Address = "931 Main St",
                    CityId = 1,
                    CountryId = 1,
                    IsPrimary = false,
                    PostArea = "Solna",
                    ZipCode = "98743"
                }
            },
            ContactInfo = new[] {
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Email,
                    Value = "test@mail.com"
                }
            }
        };
        var result = await _testBase.InsertCustomerAsync(customerCommand);

        var addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        var addressToDelete = addresses!.Last();

        var deleteResponse = await _testBase.HttpClient.DeleteAsync("/api/customeraddress/" + addressToDelete.Id);
        deleteResponse.EnsureSuccessStatusCode();

        addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        Assert.NotNull(addresses);
        Assert.Single(addresses);
        Assert.All(addresses, a => Assert.NotEqual(addressToDelete.Id, a.Id));
    }

    [Fact]
    public async Task Delete_Last_Address_Error()
    {
        await _testBase.ResetDbAsync();
        var result = await _testBase.InsertCustomerAsync();

        var addresses = await _testBase.UnitOfWork.CustomerAddressRepository.GetByCustomerIdAsync(result!.Id!.Value);
        var addressToDelete = addresses!.Last();

        var deleteResponse = await _testBase.HttpClient.DeleteAsync("/api/customeraddress/" + addressToDelete.Id);
        Assert.Equal(HttpStatusCode.BadRequest, deleteResponse.StatusCode);
    }
}