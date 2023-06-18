namespace AppLogic.Customers.CustomerPersons.Commands;

public class UpdateCustomerPersonCommand : IRequest<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
{
    public int? Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? MiddleName { get; set; }
    public string? Ssn { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateCustomerPersonValidator : AbstractValidator<UpdateCustomerPersonCommand>
    {
        public UpdateCustomerPersonValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.Ssn).NotEmpty().MinimumLength(10).MaximumLength(20).WithName("SSN");
            RuleFor(x => x.MiddleName).MaximumLength(50).WithName("Middle Name");
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }

    public class UpdateCustomerPersonHandler : IRequestHandler<UpdateCustomerPersonCommand, OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateCustomerPersonCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTimeService _dateTimeService;
        public UpdateCustomerPersonHandler(
            IUnitOfWork unitOfWork, 
            IValidator<UpdateCustomerPersonCommand> validator, 
            IAuthenticationService authenticationService,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
            _dateTimeService = dateTimeService;
        }

        public async Task<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>> Handle(
            UpdateCustomerPersonCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();

            var customer = await _unitOfWork.CustomerPersonRepository.GetByIdAsync(request.Id!.Value);
            if (customer == null)
                return new NotFound();
            if (!customer.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer has been updated by another user. Please refresh the page and try again.");
                
            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.MiddleName = request.MiddleName;
            customer.Ssn = request.Ssn;
            customer.RowVersion = request.RowVersion;
            customer.UpdatedBy = username;
            customer.Updated = _dateTimeService.GetUtc();
            
            var res = await _unitOfWork.CustomerPersonRepository.UpdateAsync(customer);
            if (res == null)
                return new Error<string>("Failed to update customer person.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }
    }
}