using Common.Services;
using Customers.CommonModels;
using Customers.BaseCustomers.Commands;
using Customers.BaseCustomers.Queries;

namespace UnitTests.Commands;

public class CustomersCommandTests
{
    [Fact]
    public async Task InsertCustomerCommand_Customer_Person()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new InsertCustomerCommand
        {
            IsCompany = false,
            FirstName = "John",
            LastName = "Doe",
            Ssn = "00000000-0000",
            MiddleName = "M",
            CustomerAddresses = new[] {
                new CustomerAddressModel {
                    Address = "123 Main St",
                    IsPrimary = true,
                    ZipCode = "12345",
                    PostArea = "Test",
                    CountryId = 1,
                    CityId = 1
                }
            },
            ContactInfo = new List<CustomerContactInfoModel> {
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Email,
                    Value = "stemih@test.com"
                },
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Phone,
                    Value = "+46701234567"
                }
            }
        };
        var handler = new InsertCustomerCommand.InsertCustomerHandler(
            mockUnitOfWork.Object,
            new InsertCustomerCommand.InsertCustomerValidator(),
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Once);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Once);
    }

    [Fact]
    public async Task InsertCustomerCommand_Customer_Company()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerCommand
        {
            IsCompany = true,
            Name = "Test Company",
            Code = "1234567890",
            CustomerAddresses = new[] {
                new CustomerAddressModel {
                    Address = "123 Main St",
                    IsPrimary = true,
                    ZipCode = "12345",
                    PostArea = "Test",
                    CountryId = 1,
                    CityId = 1
                }
            },
            ContactInfo = new List<CustomerContactInfoModel> {
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Email,
                    Value = "stemih@test.com"
                },
                new CustomerContactInfoModel {
                    Type = ContactInfoType.Phone,
                    Value = "+46701234567"
                }
            }
        };
        var handler = new InsertCustomerCommand.InsertCustomerHandler(
            mockUnitOfWork.Object,
            new InsertCustomerCommand.InsertCustomerValidator(),
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Once);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Once);
    }

    [Fact]
    public async Task InsertCustomerCommand_Validation_Error_Customer_Person()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerCommand
        {
            IsCompany = false
        };
        var handler = new InsertCustomerCommand.InsertCustomerHandler(
            mockUnitOfWork.Object,
            new InsertCustomerCommand.InsertCustomerValidator(),
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        var validationError = result.AsT2;
        Assert.True(result.IsT2);
        Assert.NotNull(validationError);
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(5, validationError.Errors.Count());
        Assert.NotNull(validationError.Errors.FirstOrDefault(_ => _.PropertyName == "Ssn"));

        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Never);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Never);
    }

    [Fact]
    public async Task InsertCustomerCommand_Validation_Error_Customer_Company()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var command = new InsertCustomerCommand
        {
            IsCompany = true
        };
        var handler = new InsertCustomerCommand.InsertCustomerHandler(
            mockUnitOfWork.Object,
            new InsertCustomerCommand.InsertCustomerValidator(),
            new TestAuthenticationService()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        var validationError = result.AsT2;
        Assert.True(result.IsT2);
        Assert.NotNull(validationError);
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(4, validationError.Errors.Count());
        Assert.NotNull(validationError.Errors.FirstOrDefault(_ => _.PropertyName == "Code"));

        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
        mockUnitOfWork.VerifyGet(x => x.CustomerContactInfoRepository, Times.Never);
        mockUnitOfWork.VerifyGet(x => x.CustomerAddressRepository, Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Person()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues
        {
            CustomerPerson = new CustomerPerson
            {
                CustomerId = 1,
                FirstName = "Test",
                LastName = "Test",
                MiddleName = "",
                Ssn = "00000000-1111",
                Id = 2
            }
        });
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            FirstName = "John",
            LastName = "Doe",
            IsCompany = false,
            Ssn = "00000000-0000",
            MiddleName = "M"
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Exactly(2));
        mockUnitOfWork.Verify(x => x.CustomerRepository.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Company()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues
        {
            CustomerCompany = new CustomerCompany
            {
                CustomerId = 1,
                Name = "Test comp",
                Code = "1234567",
                Id = 2
            }
        });
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Name = "Test",
            Code = "1234567890",
            IsCompany = true
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Exactly(2));
        mockUnitOfWork.Verify(x => x.CustomerRepository.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerCommand_No_Company_Or_Person()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Name = "Test",
            Code = "1234567890",
            IsCompany = true
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT2);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Company_With_Person_Info()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            FirstName = "Test",
            LastName = "1234567890",
            Ssn = "00000000-0000",
            MiddleName = "Test",
            IsCompany = true
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT3);
        var validationError = result.AsT3;
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(6, validationError.Errors.Count());
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "FirstName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "LastName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "MiddleName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Ssn");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Code");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Name");
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Person_With_Company_Info()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Code = "1234567890",
            Name = "Test",
            IsCompany = false
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT3);
        var validationError = result.AsT3;
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(5, validationError.Errors.Count());
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "FirstName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "LastName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Ssn");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Code");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Name");
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Validation_Error_Person()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            IsCompany = false
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT3);
        var validationError = result.AsT3;
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(4, validationError.Errors.Count());
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "FirstName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "LastName");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Ssn");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "IsActive");
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerCommand_Validation_Error_Company()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new UpdateCustomerCommand
        {
            Id = 1,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            IsCompany = true
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT3);
        var validationError = result.AsT3;
        Assert.NotEmpty(validationError.Errors);
        Assert.Equal(3, validationError.Errors.Count());
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Name");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "Code");
        Assert.Contains(validationError.Errors, _ => _.PropertyName == "IsActive");
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
    }

    [Fact]
    public async Task UpdateCustomerCommand_No_Customer()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>())).ReturnsAsync(null as Customer);

        var command = new UpdateCustomerCommand
        {
            Id = 2,
            IsActive = false,
            RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
            Name = "Test",
            Code = "1234567890",
            IsCompany = true
        };

        var handler = new UpdateCustomerCommand.UpdateCustomerHandler(
            mockUnitOfWork.Object,
            new UpdateCustomerCommand.UpdateCustomerValidator(),
            new TestAuthenticationService(),
            new DateTimeService()
        );

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.True(result.IsT1);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerCommand()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new DeleteCustomerCommand
        {
            Id = 1
        };

        var handler = new DeleteCustomerCommand.DeleteCustomerHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerCommand.DeleteCustomerValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.DeleteByIdAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCustomerCommand_Validation_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var command = new DeleteCustomerCommand();

        var handler = new DeleteCustomerCommand.DeleteCustomerHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerCommand.DeleteCustomerValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT2);
        var validationError = result.AsT2;
        Assert.NotEmpty(validationError.Errors);
        Assert.Single(validationError.Errors);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
    }

    [Fact]
    public async Task DeleteCustomerCommand_Error()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues()
        {
            DeleteCustomer = false
        });
        var command = new DeleteCustomerCommand
        {
            Id = 1
        };

        var handler = new DeleteCustomerCommand.DeleteCustomerHandler(
            mockUnitOfWork.Object,
            new DeleteCustomerCommand.DeleteCustomerValidator()
        );

        var result = await handler.Handle(command, CancellationToken.None);
        Assert.True(result.IsT1);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
    }

    [Fact]
    public async Task SearchCustomerQuery_Return_All()
    {
        var customerPerson = new Customer
        {
            Id = 1,
            CustomerPerson = new CustomerPerson
            {
                FirstName = "Test",
                LastName = "Test",
                MiddleName = "Test",
                Ssn = "00000000-0000",
                CustomerId = 1
            },
            CreatedBy = "Test",
            Created = DateTime.Now,
            UpdatedBy = "Test",
            Updated = DateTime.Now
        };
        var customerCompany = new Customer
        {
            Id = 2,
            CustomerCompany = new CustomerCompany
            {
                Name = "Test",
                Code = "Test",
                CustomerId = 2
            },
            CreatedBy = "Test",
            Created = DateTime.Now,
            UpdatedBy = "Test",
            Updated = DateTime.Now
        };
        var resultCustomers = new List<Customer> { customerPerson, customerCompany };
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        mockUnitOfWork.Setup(x => x.CustomerRepository.SearchCustomersAsync(It.IsAny<DynamicSearchQuery>()))
            .ReturnsAsync(new SearchResult<Customer>(2, resultCustomers));

        var query = new SearchCustomersQuery
        {
            SearchItems = Array.Empty<SearchItem>()
        };

        var handler = new SearchCustomersQuery.SearchCustomerHandler(
            mockUnitOfWork.Object,
            new SearchCustomersQuery.SearchCustomerValidator()
        );

        var result = await handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsT0);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Once);
        mockUnitOfWork.Verify(x => x.CustomerRepository.SearchCustomersAsync(query), Times.Once);
        var person = result.AsT0.Value.Items.First();

        Assert.Equal(customerPerson.Id, person.Id);
        Assert.Equal(customerPerson.CustomerPerson.FirstName, person.FirstName);
        Assert.Equal(customerPerson.CustomerPerson.LastName, person.LastName);
        Assert.Equal(customerPerson.CustomerPerson.MiddleName, person.MiddleName);
        Assert.Null(person.Name);
        Assert.Null(person.Code);
        Assert.False(person.IsCompany);

        var company = result.AsT0.Value.Items.Last();
        Assert.Equal(customerCompany.Id, company.Id);
        Assert.Equal(customerCompany.CustomerCompany.Name, company.Name);
        Assert.Equal(customerCompany.CustomerCompany.Code, company.Code);
        Assert.Null(company.FirstName);
        Assert.Null(company.LastName);
        Assert.Null(company.MiddleName);
        Assert.True(company.IsCompany);

        Assert.Equal(query.OrderBy, result.AsT0.Value.OrderBy);
        Assert.Equal(query.OrderByDirection, result.AsT0.Value.OrderByDirection);
        Assert.Equal(query.StartRow, result.AsT0.Value.StartRow);
        Assert.Equal(query.EndRow, result.AsT0.Value.EndRow);
        Assert.Equal(2, result.AsT0.Value.TotalCount);
    }

    [Fact]
    public async Task SearchCustomerQuery_Validation_Error_SearchItem_Name()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());
        var query = new SearchCustomersQuery
        {
            SearchItems = new[] {
                new SearchItem(
                    "Test",
                    "Test",
                    SearchOperators.Equal
                )
            }
        };

        var handler = new SearchCustomersQuery.SearchCustomerHandler(
            mockUnitOfWork.Object,
            new SearchCustomersQuery.SearchCustomerValidator()
        );

        var result = await handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsT2);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.SearchCustomersAsync(query), Times.Never);
        var validationError = result.AsT2;
        Assert.NotEmpty(validationError.Errors);
        Assert.Single(validationError.Errors);
        Assert.Equal("Search field 'Test' is not allowed.", validationError.Errors.First().ErrorMessage);
    }

    [Fact]
    public async Task SearchCustomerQuery_Validation_Error_SearchItem_Value_Empty()
    {
        var mockUnitOfWork = GetMockUnitOfWork(new RepositoryReturnValues());

        var query = new SearchCustomersQuery
        {
            SearchItems = new[] {
                new SearchItem(
                    "FirstName",
                    "",
                    SearchOperators.Equal
                )
            }
        };

        var handler = new SearchCustomersQuery.SearchCustomerHandler(
            mockUnitOfWork.Object,
            new SearchCustomersQuery.SearchCustomerValidator()
        );

        var result = await handler.Handle(query, CancellationToken.None);
        Assert.True(result.IsT2);
        mockUnitOfWork.VerifyGet(x => x.CustomerRepository, Times.Never);
        mockUnitOfWork.Verify(x => x.CustomerRepository.SearchCustomersAsync(query), Times.Never);
        var validationError = result.AsT2;
        Assert.NotEmpty(validationError.Errors);
        Assert.Single(validationError.Errors);
        Assert.Equal("Search field 'FirstName' must have a value.", validationError.Errors.First().ErrorMessage);
    }

    private record RepositoryReturnValues(
        int CustomerId = 1,
        bool CustomerContactInfo = true,
        bool CustomerAddresses = true,
        bool UpdateCustomer = true,
        bool DeleteCustomer = true,
        CustomerPerson? CustomerPerson = default,
        CustomerCompany? CustomerCompany = default
    );

    private static Mock<IUnitOfWork> GetMockUnitOfWork(RepositoryReturnValues returnValues)
    {
        var mockUnitOfWork = new Mock<IUnitOfWork>();
        var result = new SqlResult { Id = returnValues.CustomerId };
        var mockCustomerRepository = new Mock<ICustomerRepository>();
        mockCustomerRepository
            .Setup(x => x.InsertAsync(It.IsAny<Customer>()))
            .ReturnsAsync(result);
        mockCustomerRepository
            .Setup(x => x.DeleteByIdAsync(It.IsAny<int>()))
            .ReturnsAsync(returnValues.DeleteCustomer);
        mockCustomerRepository
            .Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
            .ReturnsAsync(result);
        mockCustomerRepository
            .Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>()))
            .ReturnsAsync(new Customer()
            {
                Id = returnValues.CustomerId,
                Active = true,
                RowVersion = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 },
                CustomerPerson = returnValues.CustomerPerson,
                CustomerCompany = returnValues.CustomerCompany
            });

        var mockCustomerContactInfoRepository = new Mock<ICustomerContactInfoRepository>();
        mockCustomerContactInfoRepository.
            Setup(x => x.InsertMultipleAsync(It.IsAny<IEnumerable<CustomerContactInfo>>()))
            .ReturnsAsync(returnValues.CustomerContactInfo);

        var mockCustomerAddressRepository = new Mock<ICustomerAddressRepository>();
        mockCustomerAddressRepository
            .Setup(x => x.InsertMultipleAsync(It.IsAny<IEnumerable<CustomerAddress>>()))
            .ReturnsAsync(returnValues.CustomerAddresses);

        mockUnitOfWork.SetupGet(x => x.CustomerRepository).Returns(mockCustomerRepository.Object);
        mockUnitOfWork.SetupGet(x => x.CustomerContactInfoRepository).Returns(mockCustomerContactInfoRepository.Object);
        mockUnitOfWork.SetupGet(x => x.CustomerAddressRepository).Returns(mockCustomerAddressRepository.Object);

        return mockUnitOfWork;
    }
}