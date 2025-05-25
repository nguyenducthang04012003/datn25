using PharmaDistiPro.DTO.Lots;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class LotService : ILotService
    {
        private readonly ILotRepository _lotRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LotService(ILotRepository lotRepository, IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _lotRepository = lotRepository;
        }

        public async Task<Response<LotResponse>> CreateLot(LotRequest lot)
        {
            try
            {

            var response = new Response<LotResponse>();
            var userId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);

            if (lot == null)
            {
                return new Response<LotResponse>
                {
                    StatusCode = 400,
                    Message = "Dữ liệu không hợp lệ!"
                };
            }

            // Tìm lô gần nhất để lấy số thứ tự lớn nhất
            var lastLot = await _lotRepository.GetLastLot();

            // Xác định số thứ tự tiếp theo
            int nextLotNumber = 1;
            if (lastLot != null && lastLot.LotCode.StartsWith("LOT"))
            {
                string lastCode = lastLot.LotCode.Replace("LOT", ""); // Bỏ tiền tố "LOT"
                if (int.TryParse(lastCode, out int lastNumber))
                {
                    nextLotNumber = lastNumber + 1;
                }
            }

            // Format lại LotCode với 3 chữ số (001, 002, ..., 010, 011, ...)
            string newLotCode = $"LOT{nextLotNumber:D3}";

            Lot newLot = new Lot
            {
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                LotCode = newLotCode
            };

            newLot = await _lotRepository.CreateLot(newLot);

            if (newLot != null)
            {
                return new Response<LotResponse>
                {
                    StatusCode = 200,
                    Data = new LotResponse
                    {
                        LotCode = newLot.LotCode,
                        CreatedBy = newLot.CreatedBy,
                        CreatedDate = newLot.CreatedDate
                    }
                };
            }

            return new Response<LotResponse>
            {
                StatusCode = 500,
                Message = "Lỗi khi tạo lô!"
            };
            }
            catch (Exception ex)
            {
                return new Response<LotResponse>
                {
                    StatusCode = 500,
                    Message = "Lỗi khi tạo lô! " + ex.Message
                };
            }

        }
        public async Task<Response<Lot>> GetLotByLotCode(string lotCode)
        {
            var response = new Response<Lot>();
            var lot = await _lotRepository.GetLotByLotCode(lotCode);
            if(lot == null)
            {
                response = new Response<Lot>
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy lô"
                };
                return response;
            }
            response = new Response<Lot>
            {
                StatusCode = 200,
                Data = lot
            };
            return response;
        }

        public async Task<Response<List<Lot>>> GetLotList()
        {
            var lots = await _lotRepository.GetLotList();

            

            var response = new Response<List<Lot>>();
            if (lots == null)
            {
                response = new Response<List<Lot>>
                {
                    StatusCode = 500,
                    Message = "Lỗi khi lấy dữ liệu lô!"
                };
                return response;
            }
            if(lots.Count <= 0)
            {
                response = new Response<List<Lot>>
                {
                    StatusCode = 404,
                    Message = "Không có lô nào!"
                };
                return response;
            }
            lots = lots.OrderByDescending(x => x.LotId).ToList();
            response = new Response<List<Lot>>
            {
                StatusCode = 200,

                Data = lots
            };
            return response;
        }

        public async Task<Response<LotResponse>> UpdateLot(string LotCode ,LotRequest lot)
        {
           var response = new Response<LotResponse>();
           var existed = await _lotRepository.GetLotByLotCode(LotCode);
            if (existed == null)
            {
                response = new Response<LotResponse>
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy lô"
                };
                return response;
            }


            existed.LotCode = lot.LotCode;
            existed.CreatedBy = lot.CreatedBy;
            var updatedLot = await _lotRepository.UpdateLot(existed);
            if (updatedLot != null)
            {
                LotResponse lotResponse = new LotResponse
                {
                    LotCode = updatedLot.LotCode,
                    CreatedBy = updatedLot.CreatedBy,
                    CreatedDate = updatedLot.CreatedDate
                };
                response = new Response<LotResponse>
                {
                    StatusCode = 200,
                    Data = lotResponse
                };
                return response;
            }
            response = new Response<LotResponse>
            {
                StatusCode = 500,
                Message = "Lỗi khi cập nhật lô"
            };
            return response;
        }
    }
}

