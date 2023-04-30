namespace AppLogic.Customers.CustomerPersons.Commands;

public class InsertCustomerPersonCommand : IRequest<OneOf<Success<int?>, Error<string>, ValidationError>>
{
    public int? CustomerId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Ssn { get; set; }

    public class InsertCustomerPersonValidator : AbstractValidator<InsertCustomerPersonCommand>
    {
        public InsertCustomerPersonValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.Ssn).NotEmpty().MinimumLength(10).MaximumLength(20).WithName("SSN");
            RuleFor(x => x.MiddleName).MaximumLength(50).WithName("Middle Name");
            RuleFor(x => x.CustomerId).NotEmpty();
        }
    }

    public class InsertCustomerPersonHandler : IRequestHandler<InsertCustomerPersonCommand, OneOf<Success<int?>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InsertCustomerPersonCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly CustomerMapper _mapper = new();
        public InsertCustomerPersonHandler(
            IUnitOfWork unitOfWork, 
            IValidator<InsertCustomerPersonCommand> validator, 
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
        }

        public async Task<OneOf<Success<int?>, Error<string>, ValidationError>> Handle(
            InsertCustomerPersonCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();

            var customer = _mapper.MapCommandToCustomerPersonWithParams(request, username);
            var id = await _unitOfWork.CustomerPersonRepository.InsertAsync(customer);
            if (id == null)
                return new Error<string>("Failed to insert customer person.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<int?>(id);
        }
    }
}