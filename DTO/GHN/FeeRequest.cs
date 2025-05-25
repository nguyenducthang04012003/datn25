using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace PharmaDistiPro.DTO.GHN
{

    public class FeeRequest
    {
        [JsonProperty("from_district_id")]
        [JsonPropertyName("from_district_id")]
        public int FromDistrictId { get; set; }

        [JsonProperty("from_ward_code")]
        [JsonPropertyName("from_ward_code")]
        public string FromWardCode { get; set; }


        [JsonProperty("service_type_id")]
        [JsonPropertyName("service_type_id")]
        public int ServiceTypeId { get; set; } = 2;

        [JsonProperty("to_district_id")]
        [JsonPropertyName("to_district_id")]
        public int ToDistrictId { get; set; }

        [JsonProperty("to_ward_code")]
        [JsonPropertyName("to_ward_code")]
        public string ToWardCode { get; set; }


        [JsonProperty("weight")]
        [JsonPropertyName("weight")]
        public int Weight { get; set; }


    



}
}
