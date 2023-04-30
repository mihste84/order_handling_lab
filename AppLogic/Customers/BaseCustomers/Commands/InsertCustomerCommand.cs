namespace AppLogic.Customers.BaseCustomers.Commands;

public class InsertCustomerCommand : IRequest<OneOf<Success<int?>, Error<string>, ValidationError>>
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
                RuleFor(x => x.Ssn).NotEmpty().MinimumLength(10).MaximumLength(20).WithName("SSN");
                RuleFor(x => x.MiddleName).MaximumLength(50).WithName("Middle Name");
            });

            RuleFor(x => x.CustomerAddresses).NotEmpty();
            RuleFor(x => x.ContactInfo).NotEmpty();

            RuleForEach(_ => _.CustomerAddresses).SetValidator(new CustomerAddressModelValidator());
            RuleForEach(_ => _.ContactInfo).SetValidator(new CustomerContactInfoModelValidator());
        }
    }

    public class InsertCustomerHandler : IRequestHandler<InsertCustomerCommand, OneOf<Success<int?>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InsertCustomerCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerMapper _mapper = new();
        public InsertCustomerHandler(
            IUnitOfWork unitOfWork, 
            IValidator<InsertCustomerCommand> validator, 
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
        }

        public async Task<OneOf<Success<int?>, Error<string>, ValidationError>> Handle(
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
            var customerId = await _unitOfWork.CustomerRepository.InsertAsync(customer);
            if (!customerId.HasValue) 
                return new Error<string>("Failed to create new customer.");

            var addresses = request.CustomerAddresses!.Select(_ => _mapper.MapModelToCustomerAddressWithParams(_, customerId, username));
            if (!await _unitOfWork.CustomerAddressesRepository.InsertMultipleAsync(addresses))
                return new Error<string>("Failed to create new customer addresses.");
            
            var contactInfo = request.ContactInfo!.Select(_ => _mapper.MapModelToCustomerContactInfoWithParams(_, customerId, username));                     
            if (!await _unitOfWork.CustomerContactInfoRepository.InsertMultipleAsync(contactInfo))
                return new Error<string>("Failed to create new customer contact info.");

            var id = await InsertCustomerAsync(request, customerId, username);
            if (!id.HasValue) 
                return new Error<string>("Failed to create new customer company");

            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            return new Success<int?>(customerId);
        }

        private async Task<int?> InsertCustomerAsync(InsertCustomerCommand request, int? customerId, string? username) {
            if (request.IsCompany) {
                var customerCompany = _mapper.MapCommandToCustomerCompanyWithParams(request, customerId, username);
                return await _unitOfWork.CustomerCompanyRepository.InsertAsync(customerCompany);
            } 
            var customerPerson = _mapper.MapCommandToCustomerPersonWithParams(request, customerId, username);
            return await _unitOfWork.CustomerPersonRepository.InsertAsync(customerPerson);
        }
    }
}