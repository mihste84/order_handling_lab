namespace AppLogic.Customers.CommonValidators
{
    public class CustomerContactInfoModelValidator : AbstractValidator<CustomerContactInfoModel>
    {
        public CustomerContactInfoModelValidator()
        {
            RuleFor(x => x.Type).NotEmpty();
            RuleFor(x => x.Value).NotEmpty().MaximumLength(50);
        }
    }
}