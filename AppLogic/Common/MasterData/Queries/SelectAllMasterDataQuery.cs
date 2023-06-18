using OneOf;
using OneOf.Types;

namespace AppLogic.Common.MasterData.Queries;

public class SelectAllMasterDataQuery : IRequest<OneOf<Success<MasterDataDto>>>
{
    public class SelectAllMasterDataHandler : IRequestHandler<SelectAllMasterDataQuery, OneOf<Success<MasterDataDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public SelectAllMasterDataHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OneOf<Success<MasterDataDto>>> Handle(SelectAllMasterDataQuery request, CancellationToken cancellationToken)
        {
            var countries = await _unitOfWork.CountryRepository.SelectAll();            
            var cities = await _unitOfWork.CityRepository.SelectAll();
            
            var masterDataDto = new MasterDataDto(
                countries?.Select(_ => new CountryDto(_.Id, _.Name, _.Abbreviation, _.PhonePrefix)), 
                cities?.Select(_ => new CityDto(_.Id, _.Name, _.CountryId))
            );

            return new Success<MasterDataDto>(masterDataDto);
        }
    }
}