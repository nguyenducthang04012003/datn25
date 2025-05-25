using PharmaDistiPro.DTO.Units;

namespace PharmaDistiPro.Services.Interface
{
    public interface IUnitService
    {
        Task<Services.Response<UnitDTO>> CreateNewUnits(UnitInputRequest unitInputRequest);
        Task<Services.Response<UnitDTO>> GetUnitById(int unitId);
        Task<Services.Response<IEnumerable<UnitDTO>>> GetUnitList();
        Task<Services.Response<UnitDTO>> UpdateUnit(UnitInputRequest unitUpdateRequest);
    }
}
