namespace AppLogic.Customers.CustomerPersons.Commands;

public class DeleteCustomerPersonCommand : IRequest<OneOf<Success, Error<string>, ValidationError>>
{
    public int? CustomerPersonId { get; set; }

    public class DeleteCustomerPersonValidator : AbstractValidator<DeleteCustomerPersonCommand>
    {
        public DeleteCustomerPersonValidator()
        {
            RuleFor(x => x.CustomerPersonId).NotEmpty();
        }
    }

    public class DeleteCustomerPersonHandler : IRequestHandler<DeleteCustomerPersonCommand, OneOf<Success, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteCustomerPersonCommand> _validator;

        public DeleteCustomerPersonHandler(
            IUnitOfWork unitOfWork, 
            IValidator<DeleteCustomerPersonCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success, Error<string>, ValidationError>> Handle(
            DeleteCustomerPersonCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var success = await _unitOfWork.CustomerPersonRepository.DeleteByIdAsync(request.CustomerPersonId!.Value);
            if (!success)
                return new Error<string>("Failed to delete customer person.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}