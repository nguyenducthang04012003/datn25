using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class ProvinceResponse
    {
        [JsonProperty("ProvinceID")]    
        public int ProvinceID { get; set; }

        [JsonProperty("ProvinceName")]
        public string ProvinceName { get; set; }
    }
}
