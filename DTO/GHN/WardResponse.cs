using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class WardResponse
    {
        [JsonProperty("WardCode")]
        public string WardCode { get; set; }

        [JsonProperty("WardName")]
        public string WardName { get; set; }
    }
}
