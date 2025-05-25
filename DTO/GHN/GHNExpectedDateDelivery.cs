using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class GHNExpectedDateDelivery
    {
        [JsonProperty("leadtime")]
        public long LeadTime { get; set; }

        [JsonProperty("order_date")]
        public long OrderDate { get; set; }

        public DateTime ExpectedDeliveryDate { get; set; }
        public DateTime OrderedDate { get; set; }

        public DateTime GetLeadTimeDate() => DateTimeOffset.FromUnixTimeSeconds(LeadTime).DateTime;

        public DateTime GetOrderDate() => DateTimeOffset.FromUnixTimeSeconds(OrderDate).DateTime;
    }
}
