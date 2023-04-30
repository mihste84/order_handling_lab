namespace AppLogic.Customers.CustomerContactInfos.Commands;

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
            var result = await _validator.ValidateAsync(request);
            if (!result.IsValid)
                return new ValidationError(result.Errors);

            var success = await _unitOfWork.CustomerContactInfoRepository.DeleteByIdAsync(request.Id!.Value);
            if (!success)
                return new Error<string>("Failed to delete customer contact info.");

            await _unitOfWork.SaveChangesAsync();
            
            return new Success();
        }
    }
}