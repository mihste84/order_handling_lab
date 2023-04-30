using AppLogic.Common.Validators;

namespace AppLogic.Customers.BaseCustomers.Queries;

public class SearchCustomersQuery : DynamicSearchQuery, IRequest<OneOf<Success<SearchResultDto<SearchCustomerDto>>, NotFound, ValidationError>>
{
    public class SearchCustomerValidator : AbstractValidator<SearchCustomersQuery>
    {
        private readonly string[] _allowedSearchFields = new[] { 
            "FirstName", "LastName", "MiddleName", "Code", "Name", "CountryId", "CityId", "Address", "Email", "Phone", "Active" 
        };
        public SearchCustomerValidator()
        {
            RuleFor(_ => _).SetValidator(new DynamicSearchQueryValidator(_allowedSearchFields));
        }
    }

    public class SearchCustomerHandler : IRequestHandler<SearchCustomersQuery, OneOf<Success<SearchResultDto<SearchCustomerDto>>, NotFound, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<SearchCustomersQuery> _validator;

        public SearchCustomerHandler(IUnitOfWork unitOfWork, IValidator<SearchCustomersQuery> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success<SearchResultDto<SearchCustomerDto>>, NotFound, ValidationError>> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var searchResult = await _unitOfWork.CustomerRepository.SearchCustomersAsync(request);
            if (searchResult == null || searchResult.Items?.Any() == false)
                return new NotFound();

            var mapper = new CustomerMapper();
            var dto = new SearchResultDto<SearchCustomerDto>(
                TotalCount: searchResult.TotalCount,
                Items: searchResult.Items!.Select(mapper.MapCustomerToSearchCustomerDto),
                StartRow: request.StartRow,
                EndRow: request.EndRow,
                OrderBy: request.OrderBy,
                OrderByDirection: request.OrderByDirection
            );
                        
            return new Success<SearchResultDto<SearchCustomerDto>>(dto);
        }
    }
}