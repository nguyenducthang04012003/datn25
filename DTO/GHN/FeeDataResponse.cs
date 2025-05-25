using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class FeeDataResponse
    {
        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("service_fee")]
        public int ServiceFee { get; set; }

        [JsonProperty("insurance_fee")]
        public int InsuranceFee { get; set; }

        [JsonProperty("pick_station_fee")]
        public int PickStationFee { get; set; }

        [JsonProperty("coupon_value")]
        public int CouponValue { get; set; }

        [JsonProperty("r2s_fee")]
        public int R2SFee { get; set; }

        [JsonProperty("document_return")]
        public int DocumentReturn { get; set; }

        [JsonProperty("double_check")]
        public int DoubleCheck { get; set; }

        [JsonProperty("cod_fee")]
        public int CODFee { get; set; }

        [JsonProperty("pick_remote_areas_fee")]
        public int PickRemoteAreasFee { get; set; }

        [JsonProperty("deliver_remote_areas_fee")]
        public int DeliverRemoteAreasFee { get; set; }

        [JsonProperty("cod_failed_fee")]
        public int CODFailedFee { get; set; }
    }
}
