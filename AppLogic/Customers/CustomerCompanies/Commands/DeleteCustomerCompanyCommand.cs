namespace AppLogic.Customers.CustomerCompanies.Commands;

public class DeleteCustomerCompanyCommand : IRequest<OneOf<Success, Error<string>, ValidationError>>
{
    public int? CustomerCompanyId { get; set; }

    public class DeleteCustomerCompanyValidator : AbstractValidator<DeleteCustomerCompanyCommand>
    {
        public DeleteCustomerCompanyValidator()
        {
            RuleFor(x => x.CustomerCompanyId).NotEmpty();
        }
    }

    public class DeleteCustomerCompanyHandler : IRequestHandler<DeleteCustomerCompanyCommand, OneOf<Success, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteCustomerCompanyCommand> _validator;

        public DeleteCustomerCompanyHandler(
            IUnitOfWork unitOfWork, 
            IValidator<DeleteCustomerCompanyCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success, Error<string>, ValidationError>> Handle(
            DeleteCustomerCompanyCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var success = await _unitOfWork.CustomerCompanyRepository.DeleteByIdAsync(request.CustomerCompanyId!.Value);
            if (!success)
                return new Error<string>("Failed to delete customer Company.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}