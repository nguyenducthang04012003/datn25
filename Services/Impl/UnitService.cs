using AutoMapper;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.DTO.Units;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class UnitService : IUnitService
    {
        private readonly IUnitRepository _unitRepository;
        private readonly IMapper _mapper;

        public UnitService(IUnitRepository unit, IMapper mapper)
        {
            _unitRepository = unit;
            _mapper = mapper;
        }


        // Get all unit
        public async Task<Response<IEnumerable<UnitDTO>>> GetUnitList()
        {
            var response = new Response<IEnumerable<UnitDTO>>();

            try
            {
                var units = await _unitRepository.GetAllAsync();

                if (!units.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    response.Success = true;
                    response.Data = _mapper.Map<IEnumerable<UnitDTO>>(units);
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }

            return response;
        }



        // Get unit by Id
        public async Task<Response<UnitDTO>> GetUnitById(int unitId)
        {
            var response = new Response<UnitDTO>();
            try
            {
                var units = await _unitRepository.GetByIdAsync(unitId);
                if (units == null)
                {
                    response.Success = false;
                    response.Data = null;
                    response.Message = "Không tìm thấy đơn vị thuốc";
                    return response;
                }
                else
                {
                    response.Success = true;
                    response.Data = _mapper.Map<UnitDTO>(units);
                    response.Message = "Units found";
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        //Create new Unit
        public async Task<Response<UnitDTO>> CreateNewUnits(UnitInputRequest unitInputRequest)
        {
            var response = new Response<UnitDTO>();


            try
            {
                // Kiểm tra xem unit đã tồn tại chưa 
                var existingUnit = await _unitRepository.GetSingleByConditionAsync(x => x.UnitsName.Equals(unitInputRequest.UnitsName) );
                if (existingUnit != null)
                {
                    response.Success = false;
                    response.Message = " Tên đơn vị thuốc đã tồn tại.";
                    return response;
                }



                // Map dữ liệu từ DTO sang Entity
                var newUnit = _mapper.Map<Unit>(unitInputRequest);

              newUnit.CreatedDate = DateTime.Now;

                // Thêm mới user vào database
                await _unitRepository.InsertAsync(newUnit);
                await _unitRepository.SaveAsync();

                // Trả về dữ liệu đã tạo mới
                response.Message = "Tạo mới thành công";
                response.Success = true;
                response.Data = _mapper.Map<UnitDTO>(newUnit);

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"Lỗi: {ex.Message}";
                return response;
            }
        }

       

        // Update  unit
        public async Task<Response<UnitDTO>> UpdateUnit(UnitInputRequest unitUpdateRequest)
        {
            var response = new Response<UnitDTO>();


            try
            {
                // Kiểm tra đơn vị có tồn tại không
                var unitToUpdate = await _unitRepository.GetByIdAsync(unitUpdateRequest.Id);
                if (unitToUpdate == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy đơn vị thuốc";
                    return response;
                }



                // Map dữ liệu từ DTO sang thực thể
                _mapper.Map(unitUpdateRequest,unitToUpdate);



                await _unitRepository.UpdateAsync(unitToUpdate);
                await _unitRepository.SaveAsync();

                response.Success = true;
                response.Data = _mapper.Map<UnitDTO>(unitToUpdate);
                response.Message = "Cập nhật đơn vị thuốc thành công";
            }
            catch (Exception ex)
            {

                response.Success = false;
                response.Message = "Đã xảy ra lỗi trong quá trình cập nhật đơn vị thuốc.";
            }

            return response;
        }
    }
}
