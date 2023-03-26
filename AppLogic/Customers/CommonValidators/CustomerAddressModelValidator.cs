namespace AppLogic.Customers.CommonValidators;

public class CustomerAddressModelValidator : AbstractValidator<CustomerAddressModel>
{
    public CustomerAddressModelValidator()
    {
        RuleFor(x => x.Address).NotEmpty().MaximumLength(200);
        RuleFor(x => x.PostArea).NotEmpty().MaximumLength(50);
        RuleFor(x => x.ZipCode).NotEmpty().MaximumLength(10);
        RuleFor(x => x.CountryId).NotEmpty();
        RuleFor(x => x.CityId).NotEmpty();
    }
}