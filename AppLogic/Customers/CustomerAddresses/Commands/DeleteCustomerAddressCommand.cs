using FluentValidation.Results;

namespace Customers.CustomerAddresses.Commands;

public class DeleteCustomerAddressCommand : IRequest<OneOf<Success, Error<string>, ValidationError>>
{
    public int? Id { get; set; }

    public class DeleteCustomerAddressValidator : AbstractValidator<DeleteCustomerAddressCommand>
    {
        public DeleteCustomerAddressValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteCustomerAddressHandler : IRequestHandler<DeleteCustomerAddressCommand, OneOf<Success, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteCustomerAddressCommand> _validator;

        public DeleteCustomerAddressHandler(
            IUnitOfWork unitOfWork,
            IValidator<DeleteCustomerAddressCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success, Error<string>, ValidationError>> Handle(DeleteCustomerAddressCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var addressToDelete = await _unitOfWork.CustomerAddressRepository.GetByIdAsync(request.Id!.Value);
            if (addressToDelete == null)
                return new Error<string>("Customer address not found.");

            var addressCount = await _unitOfWork.CustomerAddressRepository.GetCountByCustomerIdAsync(addressToDelete.CustomerId.GetValueOrDefault());
            if (addressCount == 1)
                return new ValidationError(new[] { new ValidationFailure("Address count", "Cannot delete the last address.") });

            var success = await _unitOfWork.CustomerAddressRepository.DeleteByIdAsync(request.Id!.Value);
            if (!success)
                return new Error<string>("Failed to delete customer address.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}