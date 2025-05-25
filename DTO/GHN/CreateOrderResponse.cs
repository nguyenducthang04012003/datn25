using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class CreateOrderResponse
    {
        [JsonProperty("order_code")]
        public string OrderCode { get; set; }

        [JsonProperty("total_fee")]
        public int TotalFee { get; set; } // Tổng phí vận chuyển
    }
}
