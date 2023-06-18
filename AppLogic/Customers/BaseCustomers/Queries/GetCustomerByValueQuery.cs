using Models.Constants;

namespace AppLogic.Customers.BaseCustomers.Queries;

public class GetCustomerByValueQuery : IRequest<OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>>
{
    public string? Type { get; set; }
    public string? Value { get; set; }

    public class GetCustomerPersonBySsnValidator : AbstractValidator<GetCustomerByValueQuery>
    {
        public GetCustomerPersonBySsnValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(20)
                .Must(type => type == "ssn" || type == "code" || type == "id")
                .WithName("Customer Type");
            RuleFor(x => x.Value)
                .MaximumLength(20)
                .NotEmpty()
                .WithName("Customer Value");

            RuleFor(_ => _).Custom((input, context) =>
            {
                if (input.Type == "id" && !int.TryParse(input.Value, out var _))
                    context.AddFailure("Customer Value", "Customer Id must be an number.");
            });
        }
    }

    public class GetCustomerPersonBySsnHandler : IRequestHandler<GetCustomerByValueQuery, OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetCustomerByValueQuery> _validator;

        public GetCustomerPersonBySsnHandler(IUnitOfWork unitOfWork, IValidator<GetCustomerByValueQuery> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>> Handle(
            GetCustomerByValueQuery request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var customer = await GetCustomerAsync(request.Type!, request.Value!) ;
            if (customer == null)
                return new NotFound();

            if (customer.CustomerPerson != null && customer.CustomerCompany == null) 
                return new Success<CustomerPersonDto>(MapToCustomerPersonDto(customer));

            if (customer.CustomerPerson == null && customer.CustomerCompany != null)
                return new Success<CustomerCompanyDto>(MapToCustomerCompanyDto(customer));

            return new Error<string>("Customer cannot be both a person and a company.");
        }

        private async Task<Customer?> GetCustomerAsync(string type, string value)
        => type switch
        {
            "ssn" => await _unitOfWork.CustomerRepository.GetBySsnAsync(value),
            "code" => await _unitOfWork.CustomerRepository.GetByCodeAsync(value),
            "id" => await _unitOfWork.CustomerRepository.GetByIdAsync(int.Parse(value)),
            _ => throw new ArgumentException("Invalid customer type", nameof(type))
        };

        private CustomerCompanyDto MapToCustomerCompanyDto(Customer model) 
        => new() {
            Active = model.Active,
            Code = model.CustomerCompany!.Code,
            Id = model.CustomerCompany.Id,
            Name = model.CustomerCompany!.Name,
            CreatedBy = model.CreatedBy,
            Created = model.Created,
            Updated = model.Updated,
            UpdatedBy = model.UpdatedBy,
            CustomerId = model.Id,
            CustomerAddresses = model.CustomerAddresses?.Select(_ => MapToCustomerAddressDto(_)),
            RowVersion = model.CustomerCompany.RowVersion,
            CustomerRowVersion = model.RowVersion,
            CustomerContactInfos = model.CustomerContactInfos?.Select(_ => MapToCustomerContactInfoDto(_)),
        };      

        private CustomerPersonDto MapToCustomerPersonDto(Customer model) 
        => new() {
            Active = model.Active,
            FirstName = model.CustomerPerson!.FirstName,
            Id = model.CustomerPerson.Id,
            LastName = model.CustomerPerson.LastName,
            MiddleName = model.CustomerPerson.MiddleName,
            Ssn = model.CustomerPerson.Ssn,
            CreatedBy = model.CreatedBy,
            Created = model.Created,
            Updated = model.Updated,
            UpdatedBy = model.UpdatedBy,
            CustomerId = model.Id,
            CustomerAddresses = model.CustomerAddresses?.Select(_ => MapToCustomerAddressDto(_)),
            RowVersion = model.CustomerPerson.RowVersion,
            CustomerRowVersion = model.RowVersion,
            CustomerContactInfos = model.CustomerContactInfos?.Select(_ => MapToCustomerContactInfoDto(_)),
        };

        private CustomerAddressDto MapToCustomerAddressDto(CustomerAddress model)
        => new(
            model.Id,
            model.CustomerId,
            model.IsPrimary,
            model.Address,
            model.PostArea,
            model.ZipCode,
            model.CountryId,
            model.CityId,
            model.CreatedBy,
            model.UpdatedBy,
            model.Created,
            model.Updated,
            model.RowVersion
        );   

        private CustomerContactInfoDto MapToCustomerContactInfoDto(CustomerContactInfo model) {
            var (value, prefix) = GetValueAndPrefix(model.Value, model.Type);
            return new(
                model.Id,
                model.CustomerId,
                model.Type,
                value,
                prefix,
                model.RowVersion
            );
        }

        private (string? Value, string? Prefix) GetValueAndPrefix(string? value, string? type) {
            if (type == ContactInfoType.Email || type == ContactInfoType.Website)
                return (value, null);

            var prefix = value?.Substring(0, 3);
            var number = value?.Substring(3);
            return (number, prefix);
        }
    }
}