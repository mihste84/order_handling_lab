namespace Customers.CustomerContactInfos.Commands;

public class DeleteCustomerContactInfoCommand : IRequest<OneOf<Success, Error<string>, ValidationError>>
{
    public int? Id { get; set; }

    public class DeleteCustomerContactInfoValidator : AbstractValidator<DeleteCustomerContactInfoCommand>
    {
        public DeleteCustomerContactInfoValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    public class DeleteCustomerContactInfoHandler : IRequestHandler<DeleteCustomerContactInfoCommand, OneOf<Success, Error<string>, ValidationError>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<DeleteCustomerContactInfoCommand> _validator;

        public DeleteCustomerContactInfoHandler(
            IUnitOfWork unitOfWork,
            IValidator<DeleteCustomerContactInfoCommand> validator)
        {
            _unitOfWork = unitOfWork;
            _validator = validator;
        }

        public async Task<OneOf<Success, Error<string>, ValidationError>> Handle(DeleteCustomerContactInfoCommand request, CancellationToken cancellationToken)
        {
            var result = await _validator.ValidateAsync(request, cancellationToken);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var infoToDelete = await _unitOfWork.CustomerContactInfoRepository.GetByIdAsync(request.Id!.Value);
            if (infoToDelete == null)
                return new Error<string>("Contact info not found.");

            var infoCount = await _unitOfWork.CustomerContactInfoRepository.GetCountByCustomerIdAsync(infoToDelete.CustomerId.GetValueOrDefault());
            if (infoCount == 1)
                return new ValidationError(new[] { new ValidationFailure("Contact info count", "Cannot delete the last contact info.") });

            var success = await _unitOfWork.CustomerContactInfoRepository.DeleteByIdAsync(request.Id!.Value);
            if (!success)
                return new Error<string>("Failed to delete customer contact info.");

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new Success();
        }
    }
}