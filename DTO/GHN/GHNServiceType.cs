using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class GHNServiceType
    {
        
            [JsonProperty("service_type_id")]
            public int ServiceTypeId { get; set; }

            [JsonProperty("short_name")]
            public string ShortName { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }
        

    }
}
