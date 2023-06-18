namespace AppLogic.Customers.CustomerAddresses.Commands;

public class UpdateCustomerAddressCommand : CustomerAddressModel, IRequest<OneOf<Success<SqlResult>, Error<string>, NotFound, ValidationError>>
{
    public int? CustomerId { get; set; }

    public class UpdateCustomerAddressValidator : AbstractValidator<UpdateCustomerAddressCommand>
    {
        public UpdateCustomerAddressValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.CustomerId).NotEmpty();
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _).SetValidator(new CustomerAddressModelValidator());
        }
    }
    
    public class UpdateCustomerAddressHandler : IRequestHandler<UpdateCustomerAddressCommand, OneOf<Success<SqlResult>, Error<string>, NotFound, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<UpdateCustomerAddressCommand> _validator;
        private readonly IDateTimeService _dateTimeService;

        public UpdateCustomerAddressHandler(
            IUnitOfWork unitOfWork, 
            IAuthenticationService authenticationService, 
            IValidator<UpdateCustomerAddressCommand> validator, 
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
            _dateTimeService = dateTimeService;
        }

        public async Task<OneOf<Success<SqlResult>, Error<string>, NotFound, ValidationError>> Handle(UpdateCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);
                
            var username = _authenticationService.GetUserName();

            var address = await _unitOfWork.CustomerAddressesRepository.GetByIdAsync(request.Id!.Value);
            if (address == null)
                return new NotFound();
            if (!address.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer address has been updated by another user. Please refresh the page and try again.");

            address.Address = request.Address;
            address.CityId = request.CityId;
            address.CountryId = request.CountryId;
            address.IsPrimary = request.IsPrimary;
            address.ZipCode = request.ZipCode;
            address.PostArea = request.PostArea;
            address.UpdatedBy = username;
            address.Updated = _dateTimeService.GetUtc();

            if (request.IsPrimary == true)
                await _unitOfWork.CustomerAddressesRepository.RemoveAllPrimaryAsync(request.CustomerId!.Value);
            
            var res = await _unitOfWork.CustomerAddressesRepository.UpdateAsync(address);
            if (res == null)
                return new Error<string>("Failed to update customer address.");

            await _unitOfWork.SaveChangesAsync();

            return new Success<SqlResult>(res);
        }
    }
}