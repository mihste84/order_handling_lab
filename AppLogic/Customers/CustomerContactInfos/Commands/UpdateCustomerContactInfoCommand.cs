namespace AppLogic.Customers.CustomerContactInfos.Commands;

public class UpdateCustomerContactInfoCommand : CustomerContactInfoModel, IRequest<OneOf<Success, NotFound, Error<string>, ValidationError>>
{ 
    public class UpdateCustomerContactInfoValidator : AbstractValidator<UpdateCustomerContactInfoCommand>
    {
        public UpdateCustomerContactInfoValidator()
        {
            RuleFor(_ => _.Id).NotEmpty();
            RuleFor(_ => _.RowVersion).NotEmpty();
            RuleFor(_ => _).SetValidator(new CustomerContactInfoModelValidator());
        }
    }

    public class UpdateCustomerContactInfoHandler : IRequestHandler<UpdateCustomerContactInfoCommand, OneOf<Success, NotFound, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<UpdateCustomerContactInfoCommand> _validator;
        private readonly IDateTimeService _dateTimeService;

        public UpdateCustomerContactInfoHandler(
            IUnitOfWork unitOfWork,
            IAuthenticationService authenticationService,
            IValidator<UpdateCustomerContactInfoCommand> validator,
            IDateTimeService dateTimeService)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
            _dateTimeService = dateTimeService;
        }

        public async Task<OneOf<Success, NotFound, Error<string>, ValidationError>> Handle(UpdateCustomerContactInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);
                
            var username = _authenticationService.GetUserName();

            var info = await _unitOfWork.CustomerContactInfoRepository.GetByIdAsync(request.Id!.Value);
            if (info == null)
                return new NotFound();
            if (!info.RowVersion!.SequenceEqual(request.RowVersion!))
                return new Error<string>("Customer contact info has been updated by another user. Please refresh the page and try again.");
            
            info.Type = request.Type;
            info.Value = request.Value;
            info.UpdatedBy = username;
            info.Updated = _dateTimeService.GetUtc();

            var success = await _unitOfWork.CustomerContactInfoRepository.UpdateAsync(info);
            if (!success)
                return new Error<string>("Failed to update customer info.");

            await _unitOfWork.SaveChangesAsync();

            return new Success();
        }
    }
}