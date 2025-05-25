using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class DistrictResponse
    {
        [JsonProperty("DistrictID")]
        public int DistrictID { get; set; }

        [JsonProperty("DistrictName")]
        public string DistrictName { get; set; }
    }
}
