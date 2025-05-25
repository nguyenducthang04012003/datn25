using Newtonsoft.Json;

namespace PharmaDistiPro.DTO.GHN
{
    public class CreateOrderRequest
    {
        [JsonProperty("payment_type_id")]
        public int PaymentTypeId { get; set; }
        [JsonProperty("from_district_id")]
        public int FromDistrictId { get; set; }

        [JsonProperty("from_ward_code")]
        public string FromWardCode { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("required_note")]
        public string RequiredNote { get; set; }

        [JsonProperty("return_phone")]
        public string ReturnPhone { get; set; }


        [JsonProperty("return_address")]
        public string ReturnAddress { get; set; }

        [JsonProperty("return_district_id")]
        public int? ReturnDistrictId { get; set; }

        [JsonProperty("return_ward_code")]
        public string ReturnWardCode { get; set; }

        [JsonProperty("client_order_code")]
        public string ClientOrderCode { get; set; }

        [JsonProperty("from_name")]
        public string FromName { get; set; }

        [JsonProperty("from_phone")]
        public string FromPhone { get; set; }

        [JsonProperty("from_address")]
        public string FromAddress { get; set; }

        [JsonProperty("from_ward_name")]
        public string FromWardName { get; set; }

        [JsonProperty("from_district_name")]
        public string FromDistrictName { get; set; }

        [JsonProperty("from_province_name")]
        public string FromProvinceName { get; set; }

        [JsonProperty("to_name")]
        public string ToName { get; set; }

        [JsonProperty("to_phone")]
        public string ToPhone { get; set; }

        [JsonProperty("to_address")]
        public string ToAddress { get; set; }

        [JsonProperty("to_ward_code")]
        public string ToWardCode { get; set; }

        [JsonProperty("to_district_id")]
        public int ToDistrictId { get; set; }

        [JsonProperty("to_province_name")]
        public string ToProvinceName { get; set; }

        [JsonProperty("cod_amount")]
        public int CODAmount { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

        [JsonProperty("cod_failed_amount")]
        public int CODFailedAmount { get; set; }

        [JsonProperty("pick_station_id")]
        public int PickStationId { get; set; } = -1;

        [JsonProperty("deliver_station_id")]
        public int? DeliverStationId { get; set; }

        [JsonProperty("insurance_value")]
        public int InsuranceValue { get; set; }

        [JsonProperty("service_type_id")]
        public int ServiceTypeId { get; set; }

        [JsonProperty("coupon")]
        public string Coupon { get; set; }

        [JsonProperty("pickup_time")]
        public long PickupTime { get; set; }

        [JsonProperty("pick_shift")]
        public List<int> PickShift { get; set; }

        [JsonProperty("items")]
        public List<OrderItem> Items { get; set; }
    }

    public class OrderItem
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("price")]
        public int Price { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }

        [JsonProperty("height")]
        public int Height { get; set; }

        [JsonProperty("weight")]
        public int Weight { get; set; }

    }

    

}
