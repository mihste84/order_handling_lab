namespace AppLogic.Customers.CustomerCompanies.Commands;

public class UpdateCustomerCompanyCommand : IRequest<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
{
    public int? Id { get; set; }
    public string? Code { get; set; }
    public string? Name { get; set; }
    public byte[]? RowVersion { get; set; }

    public class UpdateCustomerCompanyValidator : AbstractValidator<UpdateCustomerCompanyCommand>
    {
        public UpdateCustomerCompanyValidator()
        {
            RuleFor(x => x.Code).NotEmpty().WithName("Company Code");
            RuleFor(x => x.Name).NotEmpty().WithName("Company Name");
            RuleFor(x => x.Id).NotEmpty();
            RuleFor(x => x.RowVersion).NotEmpty();
        }
    }

    public class UpdateCustomerCompanyHandler : IRequestHandler<UpdateCustomerCompanyCommand, OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<UpdateCustomerCompanyCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        private readonly IDateTimeService _dateTimeService;
        public UpdateCustomerCompanyHandler(
            IUnitOfWork unitOfWork, 
            IValidator<UpdateCustomerCompanyCommand> validator, 
            IAuthenticationService authenticationService,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
            _dateTimeService = dateTimeService;
        }

        public async Task<OneOf<Success<SqlResult>, NotFound, Error<string>, ValidationError>> Handle(
            UpdateCustomerCompanyCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();

            var customer = await _unitOfWork.CustomerCompanyRepository.GetByIdAsync(request.Id!.Value);
            if (customer == null)
                return new NotFound();
            if (!customer.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer has been updated by another user. Please refresh the page and try again.");
                
            customer.Code = request.Code;
            customer.Name = request.Name;
            customer.RowVersion = request.RowVersion;
            customer.UpdatedBy = username;
            customer.Updated = _dateTimeService.GetUtc();
            
            var res = await _unitOfWork.CustomerCompanyRepository.UpdateAsync(customer);
            if (res == null)
                return new Error<string>("Failed to update customer company.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }
    }
}