using Models.Constants;

namespace Customers.CommonValidators;

public class CustomerContactInfoModelValidator : AbstractValidator<CustomerContactInfoModel>
{
    public CustomerContactInfoModelValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(x => new[] {
                ContactInfoType.Phone,
                ContactInfoType.Fax,
                ContactInfoType.Email,
                ContactInfoType.Website}.Contains(x)
            )
            .WithMessage("Invalid contact info type");
        RuleFor(x => x.Value).NotEmpty().MaximumLength(50);
        When(x => x.Type == ContactInfoType.Email, () => RuleFor(x => x.Value).EmailAddress());
        When(x => x.Type == ContactInfoType.Website,
            () => RuleFor(x => x.Value).Must(x => Uri.TryCreate(x, UriKind.Absolute, out _)));
        When(x => x.Type == ContactInfoType.Phone || x.Type == ContactInfoType.Fax,
            () => RuleFor(x => x.Value).Matches(@"^[+]\d{10,15}$").WithMessage("Invalid phone number format"));
    }
}
