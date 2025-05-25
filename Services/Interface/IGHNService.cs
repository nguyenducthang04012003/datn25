using PharmaDistiPro.DTO.GHN;

namespace PharmaDistiPro.Services.Interface
{
    public interface IGHNService
    {
        Task<Services.Response<GHNExpectedDateDelivery>> GetExpectedDateDelivery(int fromDistrictId, string fromWardCode, int toDistrictId, string toWardCode);
        Task<Services.Response<List<GHNServiceType>>> GetServiceTypes(int fromDistrictId, int toDistrictId);
        
        Task<Services.Response<CreateOrderResponse>> CreateOrder(int? orderId);
        Task<Services.Response<List<ProvinceResponse>>> GetProvinces();
        Task<Services.Response<List<DistrictResponse>>> GetDistricts(int provinceId);
        Task<Services.Response<List<WardResponse>>> GetWards(int districtId);
        Task<Response<FeeDataResponse>> CalculateShippingFee(FeeRequest requestOrder);
    }

}
