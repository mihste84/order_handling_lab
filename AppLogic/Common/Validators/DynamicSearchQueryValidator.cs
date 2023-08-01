using FluentValidation;

namespace Common.Validators;

public class DynamicSearchQueryValidator : AbstractValidator<DynamicSearchQuery>
{
    private readonly string[] _allowedOrderDirections = new[] { "ASC", "DESC" };
    public DynamicSearchQueryValidator(string[] allowedSearchFields)
    {
        RuleForEach(_ => _.SearchItems)
            .SetValidator(new SearchItemValidator(allowedSearchFields));
        RuleFor(_ => _.OrderBy)
            .NotEmpty()
            .WithName("Order By");
        RuleFor(_ => _.OrderByDirection)
            .NotEmpty()
            .Must(_ => _allowedOrderDirections.Contains(_))
            .WithName("Order By Direction");
    }
}