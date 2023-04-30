namespace AppLogic.Customers.CustomerAddresses.Commands;

public class InsertCustomerAddressCommand : CustomerAddressModel, IRequest<OneOf<Success<int>, Error<string>, ValidationError>>
{
    public int? CustomerId { get; set; }

    public class InsertCustomerAddressValidator : AbstractValidator<InsertCustomerAddressCommand>
    {
        public InsertCustomerAddressValidator()
        {
            RuleFor(x => x.CustomerId).NotNull().NotEmpty();
            RuleFor(_ => _).SetValidator(new CustomerAddressModelValidator());
        }
    }
    
    public class InsertCustomerAddressHandler : IRequestHandler<InsertCustomerAddressCommand, OneOf<Success<int>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<InsertCustomerAddressCommand> _validator;

        private readonly CustomerMapper _mapper = new CustomerMapper();

        public InsertCustomerAddressHandler(
            IUnitOfWork unitOfWork, 
            IAuthenticationService authenticationService, 
            IValidator<InsertCustomerAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
        }

        public async Task<OneOf<Success<int>, Error<string>, ValidationError>> Handle(InsertCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);
                
            var username = _authenticationService.GetUserName();

            var address = _mapper.MapModelToCustomerAddressWithParams(request, request.CustomerId, username);
            address.CustomerId = request.CustomerId;
            
            if (request.IsPrimary == true)
                await _unitOfWork.CustomerAddressesRepository.RemoveAllPrimaryAsync(request.CustomerId);
            
            var id = await _unitOfWork.CustomerAddressesRepository.InsertAsync(address);
            if (id == null)
                return new Error<string>("Failed to insert customer address.");

            await _unitOfWork.SaveChangesAsync();

            return new Success<int>(id.Value);
        }
    }
}