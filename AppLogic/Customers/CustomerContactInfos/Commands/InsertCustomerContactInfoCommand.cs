namespace AppLogic.Customers.CustomerContactInfos.Commands;

public class InsertCustomerContactInfoCommand : CustomerContactInfoModel, IRequest<OneOf<Success<int>, Error<string>, ValidationError>>
{ 
    public int? CustomerId { get; set; }

    public class InsertCustomerContactInfoValidator : AbstractValidator<InsertCustomerContactInfoCommand>
    {
        public InsertCustomerContactInfoValidator()
        {
            RuleFor(x => x.CustomerId).NotEmpty();
            RuleFor(_ => _).SetValidator(new CustomerContactInfoModelValidator());
        }
    }

    public class InsertCustomerContactInfoHandler : IRequestHandler<InsertCustomerContactInfoCommand, OneOf<Success<int>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<InsertCustomerContactInfoCommand> _validator;

        private readonly CustomerMapper _mapper = new CustomerMapper();

        public InsertCustomerContactInfoHandler(
            IUnitOfWork unitOfWork, 
            IAuthenticationService authenticationService, 
            IValidator<InsertCustomerContactInfoCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
        }

        public async Task<OneOf<Success<int>, Error<string>, ValidationError>> Handle(InsertCustomerContactInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);
                
            var username = _authenticationService.GetUserName();

            var info = _mapper.MapModelToCustomerContactInfoWithParams(request, request.CustomerId!.Value, username);
            info.CustomerId = request.CustomerId;
                   
            var id = await _unitOfWork.CustomerContactInfoRepository.InsertAsync(info);
            if (id == null)
                return new Error<string>("Failed to insert customer contact info.");

            await _unitOfWork.SaveChangesAsync();

            return new Success<int>(id.Value);
        }
    }
}
    