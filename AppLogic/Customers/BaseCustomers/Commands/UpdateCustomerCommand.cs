namespace AppLogic.Customers.BaseCustomers.Commands;

public class UpdateCustomerCommand : IRequest<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
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
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.Id!.Value);
            if (customer == null)
                return new NotFound();
            if (!customer.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer has been updated by another user. Please refresh the page and try again.");

            customer.Active = request.IsActive!.Value;
            customer.RowVersion = request.RowVersion;
            customer.UpdatedBy = _authenticationService.GetUserName();
            customer.Updated = _dateTimeService.GetUtc();

            var res = await _unitOfWork.CustomerRepository.UpdateAsync(customer);
            if (res == null)
                return new Error<string>("Failed to update customer");
                
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }
    }
}


