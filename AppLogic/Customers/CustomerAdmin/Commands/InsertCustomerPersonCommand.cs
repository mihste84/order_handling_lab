

namespace AppLogic.Customers.CustomerAdmin.Commands;

public class InsertCustomerPersonCommand : IRequest<RequestResponseDto<int>>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Ssn { get; set; }
    public string? MiddleName { get; set; }

    public IEnumerable<CustomerAddressModel>? CustomerAddresses { get; set; }
    public IEnumerable<CustomerContactInfoModel>? ContactInfo { get; set; }
    

    public class InsertCustomerPersonValidator : AbstractValidator<InsertCustomerPersonCommand>
    {
        public InsertCustomerPersonValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Ssn).NotEmpty().MaximumLength(20);
            RuleFor(x => x.MiddleName).MaximumLength(50);
            RuleFor(x => x.CustomerAddresses).NotEmpty();
            RuleFor(x => x.ContactInfo).NotEmpty();

            RuleForEach(_ => _.CustomerAddresses).SetValidator(new CustomerAddressModelValidator());
        }
    }
}