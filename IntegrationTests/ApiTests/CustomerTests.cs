using Customers.BaseCustomers.Commands;
using Customers.CommonModels;
using Customers.Constants;

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


    [Fact]
    public async Task Get_Customer_By_Ssn()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerPersonCommand();
        var response = await InsertCustomerAsync(insertCommand);

        var res = await _testBase.HttpClient.GetFromJsonAsync<CustomerDto>($"/api/customer?value={response!.Id}&type={CustomerSearchValues.Id}");

        Assert.NotNull(res);
        Assert.Equal(response.Id, res!.Id);
        Assert.Equal(insertCommand.FirstName, res.FirstName);
        Assert.Equal(insertCommand.LastName, res.LastName);
        Assert.Equal(insertCommand.MiddleName, res.MiddleName);
        Assert.Equal(insertCommand.Ssn, res.Ssn);
        Assert.NotEmpty(res.CustomerAddresses!);
        Assert.NotEmpty(res.CustomerContactInfos!);
    }

    [Fact]
    public async Task Get_Customer_By_Code()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerCompanyCommand();
        var response = await InsertCustomerAsync(insertCommand);

        var res = await _testBase.HttpClient.GetFromJsonAsync<CustomerDto>($"/api/customer?value={response!.Id}&type={CustomerSearchValues.Id}");

        Assert.NotNull(res);
        Assert.Equal(response.Id, res!.Id);
        Assert.Equal(insertCommand.Code, res.Code);
        Assert.Equal(insertCommand.Name, res.Name);
        Assert.NotEmpty(res.CustomerAddresses!);
        Assert.NotEmpty(res.CustomerContactInfos!);
    }

    [Fact]
    public async Task Get_Customer_By_Id()
    {
        await _testBase.ResetDbAsync();
        var insertCommand = GetBaseInsertCustomerPersonCommand();
        var response = await InsertCustomerAsync(insertCommand);

        var res = await _testBase.HttpClient.GetFromJsonAsync<CustomerDto>($"/api/customer?value={response!.Id}&type={CustomerSearchValues.Id}");

        Assert.NotNull(res);
        Assert.Equal(response.Id, res!.Id);
        Assert.Equal(insertCommand.FirstName, res.FirstName);
        Assert.Equal(insertCommand.LastName, res.LastName);
        Assert.Equal(insertCommand.MiddleName, res.MiddleName);
        Assert.Equal(insertCommand.Ssn, res.Ssn);
        Assert.NotEmpty(res.CustomerAddresses!);
        Assert.NotEmpty(res.CustomerContactInfos!);
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

    private async Task InsertSearchTestCustomersAsync()
    {
        var customers = new[] {
            new InsertCustomerCommand()
            {
                Name = "Company 1",
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
                        Value = "company1@mail.com"
                    },
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Phone,
                        Value = "+46701234567"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                Name = "Company 2",
                Code = "987654321",
                IsCompany = true,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "987 Main St",
                        CityId = 2,
                        CountryId = 2,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "company2@mail.com"
                    },
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Phone,
                        Value = "+46987654321"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Stefan",
                LastName = "Larsson",
                Ssn = "12345678-1234",
                IsCompany = false,
                MiddleName = "Lars",
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "654 Main St",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = true,
                        PostArea = "Solna",
                        ZipCode = "54321"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "stefan.larsson@mail.com"
                    }
                }
            },
            new InsertCustomerCommand()
            {
                FirstName = "Lars",
                LastName = "Nilsson",
                Ssn = "87654321-1234",
                IsCompany = false,
                CustomerAddresses = new[] {
                    new CustomerAddressModel {
                        Address = "XYZ St",
                        CityId = 3,
                        CountryId = 3,
                        IsPrimary = true,
                        PostArea = "ABC",
                        ZipCode = "78945"
                    },
                    new CustomerAddressModel {
                        Address = "ABC St",
                        CityId = 1,
                        CountryId = 1,
                        IsPrimary = false,
                        PostArea = "CBA",
                        ZipCode = "56431"
                    }
                },
                ContactInfo = new[] {
                    new CustomerContactInfoModel {
                        Type = ContactInfoType.Email,
                        Value = "Lars.Nilsson@mail.com"
                    }
                }
            },
        };

        await Parallel.ForEachAsync(customers, async (command, _) => await InsertCustomerAsync(command));
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