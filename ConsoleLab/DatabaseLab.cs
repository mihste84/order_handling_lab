using AppLogic.Common.Interfaces;
using AppLogic.Customers.BaseCustomers.Commands;
using AppLogic.Customers.CommonModels;
using AppLogic.Customers.Mappers;
using Models.Constants;
using Models.Entities;
using Models.Values;

namespace ConsoleLab;

public class DatabaseLab
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthenticationService _authenticationService;
    private readonly IDateTimeService _dateTimeService;
    private readonly CustomerMapper _mapper = new();

    public DatabaseLab(
        IUnitOfWork unitOfWork, 
        IAuthenticationService authenticationService, 
        IDateTimeService dateTimeService)
    {
        _unitOfWork = unitOfWork;
        _authenticationService = authenticationService;
        _dateTimeService = dateTimeService;
    }

    public async Task RunAsync()
    {
        // foreach (var cmd in GetInsertCustomerCommands()) 
        //     await InsertCustomerAsync(cmd);
        
        // await _unitOfWork.SaveChangesAsync();

        var query = new DynamicSearchQuery { 
            SearchItems = new() {
                new("CountryId", new[] { 1, 2 }, SearchOperators.In, false)
            }
        };
        var result = await _unitOfWork.CustomerRepository.SearchCustomersAsync(query);
    }

    private async Task InsertCustomerAsync(InsertCustomerCommand command) {
        var username = _authenticationService.GetUserName();
        var customer = new Customer {
            Active = true,
            CreatedBy = username,
            UpdatedBy = username
        };
        var customerId = await _unitOfWork.CustomerRepository.InsertAsync(customer);
        var addresses = command.CustomerAddresses!.Select(_ => _mapper.MapModelToCustomerAddressWithParams(_, customerId, username));
        await _unitOfWork.CustomerAddressesRepository.InsertMultipleAsync(addresses);
        
        var contactInfo = command.ContactInfo!.Select(_ => _mapper.MapModelToCustomerContactInfoWithParams(_, customerId, username));                     
        await _unitOfWork.CustomerContactInfoRepository.InsertMultipleAsync(contactInfo);
        
        if (command.IsCompany) {
            var customerCompany = _mapper.MapCommandToCustomerCompanyWithParams(command, customerId, username);
            await _unitOfWork.CustomerCompanyRepository.InsertAsync(customerCompany);
        }
        else {
            var customerPerson = _mapper.MapCommandToCustomerPersonWithParams(command, customerId, username);
            await _unitOfWork.CustomerPersonRepository.InsertAsync(customerPerson);
        }
    }

    private IEnumerable<InsertCustomerCommand> GetInsertCustomerCommands() 
    => new[] {
        new InsertCustomerCommand {
            Code = "123456789",
            Name = "Stefan Mihajlovski",
            IsCompany = true,
            CustomerAddresses = new List<CustomerAddressModel> {
                new() {
                    Address = "123 Main St",
                    CityId = 1,
                    CountryId = 1,
                    PostArea = "Stockholm",
                    IsPrimary = true,
                    ZipCode = "12345"
                }
            },
            ContactInfo = new List<CustomerContactInfoModel> {
                new() { Type = ContactInfoType.Email, Value = "stefan.mih@gmail.com" },
                new() { Type = ContactInfoType.Phone, Value = "123456789" }
            }
        },
        new InsertCustomerCommand {
            FirstName = "Stefan",
            LastName = "Mihajlovic",
            IsCompany = false,
            Ssn = "987654321",
            CustomerAddresses = new List<CustomerAddressModel> {
                new() {
                    Address = "987 Main St",
                    CityId = 1,
                    CountryId = 1,
                    PostArea = "Stockholm",
                    IsPrimary = true,
                    ZipCode = "54321"
                }
            },
            ContactInfo = new List<CustomerContactInfoModel> {
                new() { Type = ContactInfoType.Email, Value = "stefan.mihailovski@gmail.com" },
                new() { Type = ContactInfoType.Phone, Value = "987654321" }
            }
        },
        new InsertCustomerCommand {
            FirstName = "John",
            LastName = "Doe",
            IsCompany = false,
            Ssn = "123145467",
            CustomerAddresses = new List<CustomerAddressModel> {
                new() {
                    Address = "789 Main St",
                    CityId = 2,
                    CountryId = 2,
                    PostArea = "Helsinki",
                    IsPrimary = true,
                    ZipCode = "78945"
                }
            },
            ContactInfo = new List<CustomerContactInfoModel> {
                new() { Type = ContactInfoType.Email, Value = "John.Doe@gmail.com" }
            }
        }
    };
}