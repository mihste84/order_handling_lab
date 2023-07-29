namespace Customers.CommonValidators;

public class CustomerModelValidator : AbstractValidator<CustomerModel>
{
    public CustomerModelValidator()
    {
        When(_ => _.IsCompany, () =>
        {
            RuleFor(x => x.Code).NotEmpty().MaximumLength(20).WithName("Company Code");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(50).WithName("Company Name");
            RuleFor(x => x.FirstName).Empty().WithMessage("First Name must be empty when customer is company.");
            RuleFor(x => x.LastName).Empty().WithMessage("Last Name must be empty when customer is company.");
            RuleFor(x => x.Ssn).Empty().WithMessage("SSN must be empty when customer is company.");
            RuleFor(x => x.MiddleName).Empty().WithMessage("Middle Name must be empty when customer is company.");
        }).Otherwise(() =>
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50).WithName("First Name");
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50).WithName("Last Name");
            RuleFor(x => x.Ssn).NotEmpty().Matches(@"^\d{8}-[a-zA-Z0-9]{4,5}$").WithName("SSN");
            RuleFor(x => x.MiddleName).MaximumLength(50).WithName("Middle Name");
            RuleFor(x => x.Code).Empty().MaximumLength(20).WithMessage("Company Code must be empty when customer is person.");
            RuleFor(x => x.Name).Empty().MaximumLength(50).WithMessage("Comapny Name must be empty when customer is person.");
        });
    }
}