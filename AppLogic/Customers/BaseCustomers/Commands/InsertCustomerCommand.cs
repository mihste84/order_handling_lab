using Models.Exceptions;

namespace Customers.BaseCustomers.Commands;

public class InsertCustomerCommand : CustomerModel, IRequest<OneOf<Success<SqlResult>, Error<string>, ValidationError>>
{
    public IEnumerable<CustomerAddressModel>? CustomerAddresses { get; init; }
    public IEnumerable<CustomerContactInfoModel>? ContactInfo { get; init; }

    public class InsertCustomerValidator : AbstractValidator<InsertCustomerCommand>
    {
        public InsertCustomerValidator()
        {
            RuleFor(x => x).SetValidator(new CustomerModelValidator());
            RuleFor(x => x.CustomerAddresses).NotEmpty();
            RuleFor(x => x.ContactInfo).NotEmpty();

            RuleForEach(_ => _.CustomerAddresses).SetValidator(new CustomerAddressModelValidator());
            RuleForEach(_ => _.ContactInfo).SetValidator(new CustomerContactInfoModelValidator());
        }
    }

    public class InsertCustomerHandler : IRequestHandler<InsertCustomerCommand, OneOf<Success<SqlResult>, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<InsertCustomerCommand> _validator;
        private readonly IAuthenticationService _authenticationService;
        public InsertCustomerHandler(
            IUnitOfWork unitOfWork,
            IValidator<InsertCustomerCommand> validator,
            IAuthenticationService authenticationService)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
            _authenticationService = authenticationService;
        }

        public async Task<OneOf<Success<SqlResult>, Error<string>, ValidationError>> Handle(
            InsertCustomerCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var username = _authenticationService.GetUserName();

            var customer = new Customer
            {
                Active = true,
                CreatedBy = username,
                UpdatedBy = username,
                CustomerPerson = MapModelToCustomerPerson(request),
                CustomerCompany = MapModelToCustomerCompany(request)
            };

            try
            {
                var customerRes = await _unitOfWork.CustomerRepository.InsertAsync(customer);
                if (customerRes == null)
                    return new Error<string>("Failed to create new customer.");

                var addresses = request.CustomerAddresses!.Select(_ => MapModelToCustomerAddress(_, customerRes.Id, username));
                if (!await _unitOfWork.CustomerAddressRepository.InsertMultipleAsync(addresses))
                    return new Error<string>("Failed to create new customer addresses.");

                var contactInfo = request.ContactInfo!.Select(_ => MapModelToCustomerContactInfo(_, customerRes.Id, username));
                if (!await _unitOfWork.CustomerContactInfoRepository.InsertMultipleAsync(contactInfo))
                    return new Error<string>("Failed to create new customer contact info.");

                await _unitOfWork.SaveChangesAsync(CancellationToken.None);
                return new Success<SqlResult>(customerRes);
            }
            catch (UniqueConstraintException ex)
            {
                return new Error<string>(ex.Message);
            }
        }

        private static CustomerPerson? MapModelToCustomerPerson(InsertCustomerCommand model)
        => !model.IsCompany ? new()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            MiddleName = model.MiddleName,
            Ssn = model.Ssn
        } : default;

        private static CustomerCompany? MapModelToCustomerCompany(InsertCustomerCommand model)
        => model.IsCompany ? new()
        {
            Name = model.Name,
            Code = model.Code
        } : default;

        private static CustomerContactInfo MapModelToCustomerContactInfo(CustomerContactInfoModel model, int? customerId, string? username)
        => new()
        {
            Type = model.Type,
            Value = model.Value,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId
        };

        private static CustomerAddress MapModelToCustomerAddress(CustomerAddressModel model, int? customerId, string? username)
        => new()
        {
            Address = model.Address,
            CityId = model.CityId,
            CountryId = model.CountryId,
            ZipCode = model.ZipCode,
            CreatedBy = username,
            UpdatedBy = username,
            CustomerId = customerId,
            PostArea = model.PostArea,
            IsPrimary = model.IsPrimary
        };
    }
}