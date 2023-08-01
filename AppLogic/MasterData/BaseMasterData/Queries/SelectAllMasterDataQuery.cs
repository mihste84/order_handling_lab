namespace MasterData.BaseMasterData.Queries;

public class SelectAllMasterDataQuery : IRequest<OneOf<Success<MasterDataDto>, NotFound>>
{
    public class SelectAllMasterDataHandler : IRequestHandler<SelectAllMasterDataQuery, OneOf<Success<MasterDataDto>, NotFound>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public SelectAllMasterDataHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OneOf<Success<MasterDataDto>, NotFound>> Handle(SelectAllMasterDataQuery request, CancellationToken cancellationToken)
        {
            var (Cities, Countries) = await _unitOfWork.CustomerAddressRepository.GetAllReferenceDataAsync();
            if (Cities == null || Countries == null)
                return new NotFound();

            var masterDataDto = new MasterDataDto(
                Countries?.Select(_ => new CountryDto(_.Id, _.Name, _.Abbreviation, _.PhonePrefix)),
                Cities?.Select(_ => new CityDto(_.Id, _.Name, _.CountryId))
            );

            return new Success<MasterDataDto>(masterDataDto);
        }
    }
}