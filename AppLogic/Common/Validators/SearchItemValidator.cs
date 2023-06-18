using FluentValidation;

namespace AppLogic.Common.Validators;

public class SearchItemValidator : AbstractValidator<SearchItem>
{
    public SearchItemValidator(string[] _allowedSearchFields)
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithName("Search Item Name")
            .Must(_ => _allowedSearchFields.Contains(_))
            .WithMessage(_ => $"Search field '{_.Name}' is not allowed.");
        RuleFor(x => x.Value)
            .NotEmpty()
            .When(x => x.Operator != SearchOperators.IsNull && x.Operator != SearchOperators.IsNotNull)
            .WithName("Search Item Value")
            .WithMessage(_ => $"Search field '{_.Name}' must have a value.");        
        RuleFor(x => x.Operator)
            .NotEmpty()
            .WithName("Search Item Operator");
        RuleFor(_ => _.HandleAutomatically).NotNull().WithName("Handle Automatically");
    }
}