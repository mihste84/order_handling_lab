using AppLogic.Customers.BaseCustomers.Commands;
using AppLogic.Customers.CustomerCompanies.Commands;
using AppLogic.Customers.CustomerPersons.Commands;
using Riok.Mapperly.Abstractions;

namespace AppLogic.Customers.Mappers;

[Mapper]
public partial class CustomerMapper
{
    public partial CustomerAddress MapModelToCustomerAddress(CustomerAddressModel model);
    public partial CustomerContactInfo MapModelToCustomerContactInfo(CustomerContactInfoModel model);
    public partial CustomerPerson MapCommandToCustomerPerson(InsertCustomerCommand model);
    public partial CustomerPerson MapCommandToCustomerPerson(InsertCustomerPersonCommand model);
    public partial CustomerCompany MapCommandToCustomerCompany(InsertCustomerCommand model);
    public partial CustomerCompany MapCommandToCustomerCompany(InsertCustomerCompanyCommand model);

    
    [MapProperty(new[] { nameof(Customer.CustomerPerson), nameof(Customer.CustomerPerson.FirstName) }, new[] { nameof(SearchCustomerDto.FirstName) })]
    [MapProperty(new[] { nameof(Customer.CustomerPerson), nameof(Customer.CustomerPerson.LastName) }, new[] { nameof(SearchCustomerDto.LastName) })]
    [MapProperty(new[] { nameof(Customer.CustomerPerson), nameof(Customer.CustomerPerson.MiddleName) }, new[] { nameof(SearchCustomerDto.MiddleName) })]
    [MapProperty(new[] { nameof(Customer.CustomerPerson), nameof(Customer.CustomerPerson.Ssn) }, new[] { nameof(SearchCustomerDto.Ssn) })]
    [MapProperty(new[] { nameof(Customer.CustomerCompany), nameof(Customer.CustomerCompany.Name) }, new[] { nameof(SearchCustomerDto.Name) })]
    [MapProperty(new[] { nameof(Customer.CustomerCompany), nameof(Customer.CustomerCompany.Code) }, new[] { nameof(SearchCustomerDto.Code) })]
    public partial SearchCustomerDto MapCustomerToSearchCustomer(Customer customer);
    public partial CustomerCompanyDto MapCustomerCompanyToCustomerDto(Customer customer);
    public partial CustomerPersonDto MapCustomerPersonToCustomerDto(Customer customer);
    public partial CustomerAddressDto MapCustomerAddressToCustomerAddressDto(CustomerAddress customerAddress);
    public partial CustomerContactInfoDto MapCustomerContactInfoToCustomerContactInfoDto(CustomerContactInfo customerContactInfo);

    public SearchCustomerDto MapCustomerToSearchCustomerDto(Customer customer)
    {
        var dto = MapCustomerToSearchCustomer(customer);
        dto.IsCompany = customer.CustomerCompany != null;
        return dto;
    }

    public CustomerAddress MapModelToCustomerAddressWithParams(CustomerAddressModel model, int? customerId, string? username)
    {
        var address = MapModelToCustomerAddress(model);
        address.CustomerId = customerId;
        address.CreatedBy = username;
        address.UpdatedBy = username;
        return address;
    }

    public CustomerContactInfo MapModelToCustomerContactInfoWithParams(CustomerContactInfoModel model, int? customerId, string? username)
    {
        var contactInfo = MapModelToCustomerContactInfo(model);
        contactInfo.CustomerId = customerId;
        contactInfo.CreatedBy = username;
        contactInfo.UpdatedBy = username;
        return contactInfo;
    }

    public CustomerPerson MapCommandToCustomerPersonWithParams(InsertCustomerCommand model, int? customerId, string? username)
    {
        var customer = MapCommandToCustomerPerson(model);
        customer.CustomerId = customerId;
        customer.CreatedBy = username;
        customer.UpdatedBy = username;
        return customer;
    }

    public CustomerPerson MapCommandToCustomerPersonWithParams(InsertCustomerPersonCommand model, string? username)
    {
        var customer = MapCommandToCustomerPerson(model);
        customer.CreatedBy = username;
        customer.UpdatedBy = username;
        return customer;
    }

    public CustomerCompany MapCommandToCustomerCompanyWithParams(InsertCustomerCommand model, int? customerId, string? username)
    {
        var customer = MapCommandToCustomerCompany(model);
        customer.CustomerId = customerId;
        customer.CreatedBy = username;
        customer.UpdatedBy = username;
        return customer;
    }

    public CustomerCompany MapCommandToCustomerCompanyWithParams(InsertCustomerCompanyCommand model, string? username)
    {
        var customer = MapCommandToCustomerCompany(model);
        customer.CreatedBy = username;
        customer.UpdatedBy = username;
        return customer;
    }
}