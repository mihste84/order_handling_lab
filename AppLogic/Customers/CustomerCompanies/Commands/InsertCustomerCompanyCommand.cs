namespace AppLogic.Customers.CustomerCompanies.Commands;

public class InsertCustomerCompanyCommand : IRequest<OneOf<Success<int?>, Error<string>, ValidationError>>
{
    public int? CustomerId { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public class InsertCustomerCompanyValidator : AbstractValidator<InsertCustomerCompanyCommand>
    {
        public InsertCustomerCompanyValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithName("Company Code");
            RuleFor(x => x.Name).NotEmpty().WithName("Company Name");
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }

    public class InsertCustomerCompanyHandler : IRequestHandler<InsertCustomerCompanyCommand, OneOf<Success<int?>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InsertCustomerCompanyCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerMapper _mapper = new();
        public InsertCustomerCompanyHandler(
            IUnitOfWork unitOfWork, 
            IValidator<InsertCustomerCompanyCommand> validator, 
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
        }

        public async Task<OneOf<Success<int?>, Error<string>, ValidationError>> Handle(
            InsertCustomerCompanyCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();

            var customer = _mapper.MapCommandToCustomerCompanyWithParams(request, username);
            var id = await _unitOfWork.CustomerCompanyRepository.InsertAsync(customer);
            if (id == null)
                return new Error<string>("Failed to insert customer company.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<int?>(id);
        }
    }
}