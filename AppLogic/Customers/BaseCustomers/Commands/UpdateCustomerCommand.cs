namespace Customers.BaseCustomers.Commands;

public class UpdateCustomerCommand : CustomerModel, IRequest<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
{
    public int? Id { get; set; }
    public bool? IsActive { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateCustomerValidator : AbstractValidator<UpdateCustomerCommand>
    {
        public UpdateCustomerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.IsActive).NotEmpty().WithName("Is active");
            RuleFor(x => x.RowVersion).NotEmpty();
            RuleFor(x => x).SetValidator(new CustomerModelValidator());
        }
    }

    public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateCustomerCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTimeService _dateTimeService;
        public UpdateCustomerHandler(
            IUnitOfWork unitOfWork,
            IValidator<UpdateCustomerCommand> validator,
            IAuthenticationService authenticationService,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
            _dateTimeService = dateTimeService;
        }
        public async Task<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>> Handle(
            UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.Id!.Value, false);
            if (customer == null)
                return new NotFound();
            if (!customer.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer has been updated by another user. Please refresh the page and try again.");

            customer.Active = request.IsActive!.Value;
            customer.RowVersion = request.RowVersion;
            customer.UpdatedBy = _authenticationService.GetUserName();
            customer.Updated = _dateTimeService.GetUtc();
            if (customer.CustomerPerson != null)
            {
                customer.CustomerPerson.FirstName = request.FirstName;
                customer.CustomerPerson.LastName = request.LastName;
                customer.CustomerPerson.MiddleName = request.MiddleName;
                customer.CustomerPerson.Ssn = request.Ssn;
            }
            else if (customer.CustomerCompany != null)
            {
                customer.CustomerCompany.Code = request.Code;
                customer.CustomerCompany.Name = request.Name;
            }
            else
            {
                return new Error<string>("Customer is neither a person nor a company");
            }

            var res = await _unitOfWork.CustomerRepository.UpdateAsync(customer);
            if (res == null)
                return new Error<string>("Failed to update customer");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }
    }
}