namespace AppLogic.Customers.BaseCustomers.Queries;

public class GetCustomerByValueQuery : IRequest<OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>>
{
    public string? Type { get; set; }
    public string? Value { get; set; }

    public class GetCustomerPersonBySsnValidator : AbstractValidator<GetCustomerByValueQuery>
    {
        public GetCustomerPersonBySsnValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(20)
                .Must(type => type == "ssn" || type == "code" || type == "id")
                .WithName("Customer Type");
            RuleFor(x => x.Value)
                .MaximumLength(20)
                .NotEmpty()
                .WithName("Customer Value");

            RuleFor(_ => _).Custom((input, context) =>
            {
                if (input.Type == "id" && !int.TryParse(input.Value, out var _))
                    context.AddFailure("Customer Value", "Customer Id must be an number.");
            });
        }
    }

    public class GetCustomerPersonBySsnHandler : IRequestHandler<GetCustomerByValueQuery, OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<GetCustomerByValueQuery> _validator;

        public GetCustomerPersonBySsnHandler(IUnitOfWork unitOfWork, IValidator<GetCustomerByValueQuery> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success<CustomerPersonDto>, Success<CustomerCompanyDto>, Error<string>, NotFound, ValidationError>> Handle(
            GetCustomerByValueQuery request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var customer = await GetCustomerAsync(request.Type!, request.Value!) ;
            if (customer == null)
                return new NotFound();

            var mapper = new CustomerMapper();
            if (customer.CustomerPerson != null && customer.CustomerCompany == null) 
                return new Success<CustomerPersonDto>(mapper.MapCustomerPersonToCustomerDto(customer));

            if (customer.CustomerPerson == null && customer.CustomerCompany != null)
                return new Success<CustomerCompanyDto>(mapper.MapCustomerCompanyToCustomerDto(customer));

            return new Error<string>("Customer cannot be both a person and a company.");
        }

        private async Task<Customer?> GetCustomerAsync(string type, string value)
        => type switch
        {
            "ssn" => await _unitOfWork.CustomerRepository.GetBySsnAsync(value),
            "code" => await _unitOfWork.CustomerRepository.GetByCodeAsync(value),
            "id" => await _unitOfWork.CustomerRepository.GetByIdAsync(int.Parse(value)),
            _ => throw new ArgumentException("Invalid customer type", nameof(type))
        };
    }
}