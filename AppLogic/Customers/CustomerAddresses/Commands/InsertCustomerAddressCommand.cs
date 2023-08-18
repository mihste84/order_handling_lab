using FluentValidation.Results;

namespace Customers.CustomerAddresses.Commands;

public class InsertCustomerAddressCommand : CustomerAddressModel, IRequest<OneOf<Success<SqlResult>, Error<string>, ValidationError>>
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

    public class InsertCustomerAddressHandler : IRequestHandler<InsertCustomerAddressCommand, OneOf<Success<SqlResult>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public readonly IAuthenticationService _authenticationService;
        private readonly IValidator<InsertCustomerAddressCommand> _validator;

        public InsertCustomerAddressHandler(
            IUnitOfWork unitOfWork,
            IAuthenticationService authenticationService,
            IValidator<InsertCustomerAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _authenticationService = authenticationService;
            _validator = validator;
        }

        public async Task<OneOf<Success<SqlResult>, Error<string>, ValidationError>> Handle(InsertCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var addressCount = await _unitOfWork.CustomerAddressRepository.GetCountByCustomerIdAsync(request.CustomerId.GetValueOrDefault());
            if (addressCount == 3)
                return new ValidationError(new[] { new ValidationFailure("Address count", "Customer cannot have more than 3 different addresses.") });

            var username = _authenticationService.GetUserName();

            var address = MapModelToCustomerAddress(request, username);

            if (request.IsPrimary == true)
                await _unitOfWork.CustomerAddressRepository.RemoveAllPrimaryAsync(request.CustomerId);

            var res = await _unitOfWork.CustomerAddressRepository.InsertAsync(address);
            if (res == null)
                return new Error<string>("Failed to insert customer address.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success<SqlResult>(res);
        }

        private static CustomerAddress MapModelToCustomerAddress(InsertCustomerAddressCommand model, string username)
        => new()
        {
            Id = model.Id,
            CustomerId = model.CustomerId,
            IsPrimary = model.IsPrimary,
            Address = model.Address,
            PostArea = model.PostArea,
            ZipCode = model.ZipCode,
            CountryId = model.CountryId,
            CityId = model.CityId,
            CreatedBy = username,
            UpdatedBy = username,
            RowVersion = model.RowVersion
        };
    }
}