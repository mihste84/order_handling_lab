namespace Customers.BaseCustomers.Commands;

public class DeleteCustomerCommand : IRequest<OneOf<Success, NotFound, ValidationError>>
{
    public int? Id { get; set; }

    public class DeleteCustomerValidator : AbstractValidator<DeleteCustomerCommand>
    {
        public DeleteCustomerValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteCustomerHandler : IRequestHandler<DeleteCustomerCommand, OneOf<Success, NotFound, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteCustomerCommand> _validator;
        public DeleteCustomerHandler(
            IUnitOfWork unitOfWork,
            IValidator<DeleteCustomerCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }
        public async Task<OneOf<Success, NotFound, ValidationError>> Handle(
            DeleteCustomerCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var succeess = await _unitOfWork.CustomerRepository.DeleteByIdAsync(request.Id!.Value);
            if (!succeess)
                return new NotFound();

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}