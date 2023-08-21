namespace Customers.CustomerContactInfos.Commands;

public class InsertCustomerContactInfoCommand : CustomerContactInfoModel, IRequest<OneOf<Success<SqlResult>, Error<string>, ValidationError>>
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

    public class InsertCustomerContactInfoHandler : IRequestHandler<InsertCustomerContactInfoCommand, OneOf<Success<SqlResult>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<InsertCustomerContactInfoCommand> _validator;

        public InsertCustomerContactInfoHandler(
            IUnitOfWork unitOfWork,
            IAuthenticationService authenticationService,
            IValidator<InsertCustomerContactInfoCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
        }

        public async Task<OneOf<Success<SqlResult>, Error<string>, ValidationError>> Handle(InsertCustomerContactInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var infoCount = await _unitOfWork.CustomerContactInfoRepository.GetCountByCustomerIdAsync(request.CustomerId!.Value);
            if (infoCount >= 5)
                return new ValidationError(new[] { new ValidationFailure("Contact info count", "Customer cannot have more than 5 contact info.") });

            var username = _authenticationService.GetUserName();

            var info = MapModelToCustomerContactInfo(request, username);

            var res = await _unitOfWork.CustomerContactInfoRepository.InsertAsync(info);
            if (res == null)
                return new Error<string>("Failed to insert customer contact info.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }

        private static CustomerContactInfo MapModelToCustomerContactInfo(InsertCustomerContactInfoCommand model, string username)
        => new()
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            Type = model.Type,
            Value = model.Value,
            CreatedBy = username,
            UpdatedBy = username
        };
    }
}
