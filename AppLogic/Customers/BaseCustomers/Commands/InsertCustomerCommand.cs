using Models.Exceptions;

namespace AppLogic.Customers.BaseCustomers.Commands;

public class InsertCustomerCommand : IRequest<OneOf<Success<SqlResult>, Error<string>, ValidationError>>
{
    public bool IsCompany { get; init; }
    public string? Code { get; init; }
    public string? Name { get; init; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Ssn { get; set; }
    public string? MiddleName { get; set; }

    public IEnumerable<CustomerAddressModel>? CustomerAddresses { get; init; }
    public IEnumerable<CustomerContactInfoModel>? ContactInfo { get; init; }

    public class InsertCustomerValidator : AbstractValidator<InsertCustomerCommand>
    {
        public InsertCustomerValidator()
        {
            When(_ => _.IsCompany, () => {
                RuleFor(x => x.Code).NotEmpty().MaximumLength(20).WithName("Company Code");;
                RuleFor(x => x.Name).NotEmpty().MaximumLength(50).WithName("Company Name");
            }).Otherwise(() => {
                RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
                RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
                RuleFor(x => x.Ssn).NotEmpty().Matches(@"^\d{8}-[a-zA-Z0-9]{4,5}$").WithName("SSN");
                RuleFor(x => x.MiddleName).MaximumLength(50).WithName("Middle Name");
            });

            RuleFor(x => x.CustomerAddresses).NotEmpty();
            RuleFor(x => x.ContactInfo).NotEmpty();

            RuleForEach(_ => _.CustomerAddresses).SetValidator(new CustomerAddressModelValidator());
            RuleForEach(_ => _.ContactInfo).SetValidator(new CustomerContactInfoModelValidator());
        }
    }

    public class InsertCustomerHandler : IRequestHandler<InsertCustomerCommand, OneOf<Success<SqlResult>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InsertCustomerCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        public InsertCustomerHandler(
            IUnitOfWork unitOfWork, 
            IValidator<InsertCustomerCommand> validator, 
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
        }

        public async Task<OneOf<Success<SqlResult>, Error<string>, ValidationError>> Handle(
            InsertCustomerCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();
            
            var customer = new Customer {
                Active = true,
                CreatedBy = username,
                UpdatedBy = username
            };
            var customerRes = await _unitOfWork.CustomerRepository.InsertAsync(customer);
            if (customerRes == null) 
                return new Error<string>("Failed to create new customer.");

            var addresses = request.CustomerAddresses!.Select(_ => MapModelToCustomerAddress(_, customerRes.Id, username));
            if (!await _unitOfWork.CustomerAddressRepository.InsertMultipleAsync(addresses))
                return new Error<string>("Failed to create new customer addresses.");
            
            var contactInfo = request.ContactInfo!.Select(_ => MapModelToCustomerContactInfo(_, customerRes.Id, username));                     
            if (!await _unitOfWork.CustomerContactInfoRepository.InsertMultipleAsync(contactInfo))
                return new Error<string>("Failed to create new customer contact info.");

            try {
                var res = await InsertCustomerAsync(request, customerRes.Id, username);
                if (res == null) 
                    return new Error<string>("Failed to create new customer company");

                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                return new Success<SqlResult>(customerRes);
            } catch (UniqueConstraintException ex) {
                return new Error<string>(ex.Message);
            }    
        }

        private async Task<SqlResult?> InsertCustomerAsync(InsertCustomerCommand request, int? customerId, string? username) {
            if (request.IsCompany) {
                var customerCompany = MapModelToCustomerCompany(request, customerId, username);
                return await _unitOfWork.CustomerCompanyRepository.InsertAsync(customerCompany);
            } 
            var customerPerson = MapModelToCustomerPerson(request, customerId, username);
            return await _unitOfWork.CustomerPersonRepository.InsertAsync(customerPerson);
        }

        private CustomerPerson MapModelToCustomerPerson(InsertCustomerCommand model, int? customerId, string? username) 
        => new() {
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            Ssn = model.Ssn,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId
        };

        private CustomerCompany MapModelToCustomerCompany(InsertCustomerCommand model, int? customerId, string? username) 
        => new() {
            Name = model.Name,
            Code = model.Code,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId
        };

        private CustomerContactInfo MapModelToCustomerContactInfo(CustomerContactInfoModel model, int? customerId, string? username)
        => new() {
            Type = model.Type,
            Value = model.Value,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId
        };

        private CustomerAddress MapModelToCustomerAddress(CustomerAddressModel model, int? customerId, string? username) 
        => new() {
            Address = model.Address,
            CityId = model.CityId,
            CountryId = model.CountryId,
            ZipCode = model.ZipCode,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId,
            PostArea = model.PostArea,
            IsPrimary = model.IsPrimary
        };

    }
}