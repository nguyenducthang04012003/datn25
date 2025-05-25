using Newtonsoft.Json;
using PharmaDistiPro.Services.Interface;
using System.Text;
using PharmaDistiPro.DTO.GHN;
using PharmaDistiPro.Models;
using System.Runtime.CompilerServices;
using PharmaDistiPro.Repositories.Interface;
namespace PharmaDistiPro.Services.Impl
{
    public class GHNService : IGHNService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiUrl = "https://dev-online-gateway.ghn.vn/shiip/public-api";
        private readonly string _token;
        private readonly string _shopId;
        private readonly string _returnPhone, _returnAddress, _returnDistrictId, _returnWardCode;
        private readonly string _fromName, _fromWardName, _fromDistrictName, _fromProvinceName;

        private readonly IOrderRepository _orderRepository;
        private readonly IOrdersDetailRepository _orderDetailRepository;

        public GHNService(HttpClient httpClient, IConfiguration configuration, IOrderRepository orderRepository, IOrdersDetailRepository ordersDetailRepository)
        {
            _httpClient = httpClient;
            _token = configuration["GHN:Token"]; 
            _shopId = configuration["GHN:ShopId"];
            _returnPhone = configuration["GHN:returnPhone"];
            _returnAddress = configuration["GHN:returnAddress"];
            _returnDistrictId = configuration["GHN:returnDistrictId"];
            _returnWardCode = configuration["GHN:returnWardCode"];
            _fromName = configuration["GHN:from_name"];
            _fromWardName = configuration["GHN:from_ward_name"];
            _fromDistrictName = configuration["GHN:from_district_name"];
            _fromProvinceName = configuration["GHN:from_province_name"]; 
            _orderRepository = orderRepository;
            _orderDetailRepository = ordersDetailRepository;
        }

        private void AddHeaders(HttpRequestMessage request)
        {
            request.Headers.Add("Token", _token);
            if (!string.IsNullOrEmpty(_shopId))
            {
                request.Headers.Add("ShopId", _shopId);
            }
        }

        #region get Addess

        public async Task<Response<List<ProvinceResponse>>> GetProvinces()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/master-data/province");
            AddHeaders(request);

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new Response<List<ProvinceResponse>>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "Lỗi từ API GHN",
                    Data = null
                };
            }

            // Deserialize vào GHNResponse<List<ProvinceResponse>>
            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<List<ProvinceResponse>>>(content);

            return new Response<List<ProvinceResponse>>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
        }

        public async Task<Response<List<DistrictResponse>>> GetDistricts(int provinceId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/master-data/district");
            AddHeaders(request);
            var body = new { province_id = provinceId };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var responseReturn = new Response<List<DistrictResponse>>();

            if(!response.IsSuccessStatusCode)
            {
                responseReturn = new Response<List<DistrictResponse>>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "Lỗi từ API GHN",
                    Data = null
                };
                return responseReturn;
            }

            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<List<DistrictResponse>>>(content);
            responseReturn = new Response<List<DistrictResponse>>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };

            return responseReturn;
        }

        public async Task<Response<List<WardResponse>>> GetWards(int districtId)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_apiUrl}/master-data/ward");
            AddHeaders(request);
            var body = new { district_id = districtId };
            request.Content = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var responseReturn = new Response<List<WardResponse>>();
            if(!response.IsSuccessStatusCode)
            {
                responseReturn = new Response<List<WardResponse>>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = "Lỗi từ API GHN",
                    Data = null
                };
                return responseReturn;
            }
            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<List<WardResponse>>>(content);
            responseReturn = new Response<List<WardResponse>>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
            return responseReturn;
        }
        #endregion

        #region Create Order

        public async Task<Response<CreateOrderResponse>> CreateOrder(int? orderId)
        {

            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/v2/shipping-order/create");
            AddHeaders(request);

            var orders = await _orderRepository.GetSingleByConditionAsync(x => x.OrderId == orderId, includes:new string[] { "Customer" });

            if (orders == null)
            {
                return new Response<CreateOrderResponse>()
                {
                    StatusCode = 404,
                    Message = "Không tìm thấy đơn hàng",
                    Data = null
                };
            }



            var singleOrderDetail = await _orderDetailRepository.GetSingleByConditionAsync(
              x => x.OrderId == orderId,
     includes: new string[] { "Product" });

            // Wrap in a list if it's not null
            List<OrdersDetail> orderDetails = singleOrderDetail != null ? new List<OrdersDetail> { singleOrderDetail } : new List<OrdersDetail>();



            CreateOrderRequest orderRequest = new CreateOrderRequest();
            orderRequest.PaymentTypeId = 1;
            orderRequest.Note = "TEST ANH LY DUC TRUNG HIEU";
            orderRequest.ReturnDistrictId = int.Parse(_returnDistrictId);
            // return info
            orderRequest.ReturnWardCode =  _returnWardCode;
            orderRequest.ReturnAddress = _returnAddress;
            orderRequest.ReturnPhone = _returnPhone;
            orderRequest.RequiredNote = "KHONGCHOXEMHANG";
            orderRequest.CODAmount = 0;
            orderRequest.InsuranceValue = 0;
            orderRequest.ServiceTypeId = 2;
            orderRequest.Content = "Content TEST API GHN";
            // From info
            orderRequest.FromName = _fromName;
            orderRequest.FromPhone = _returnPhone;
            orderRequest.FromAddress = _returnAddress;
            orderRequest.FromWardName = _fromWardName;
            orderRequest.FromDistrictName = _fromDistrictName;
            orderRequest.FromProvinceName = _fromProvinceName;
            
            // Customer info
            orderRequest.ToName = orders.Customer.FirstName + " " + orders.Customer.LastName;
            orderRequest.ToPhone = orders.Customer.Phone;
            orderRequest.ToAddress = orders.Customer.Address;
            orderRequest.ToDistrictId = orders.DistrictId.Value;
            orderRequest.ToWardCode = orders.WardCode;
            orderRequest.ClientOrderCode = orders.OrderCode;
            orderRequest.Coupon = "";
            // order info
            orderRequest.Weight = (int)(orderDetails.Sum(s=>s.Product.Weight * s.Quantity)*1000);
            List<OrderItem> listItem = new List<OrderItem>();
            foreach (var orderDetail in orderDetails)
            {
                OrderItem item = new OrderItem();
                item.Name = orderDetail.Product.ProductName;
                item.Code = orderDetail.Product.ProductCode;
                item.Quantity = (int)orderDetail.Quantity;
                item.Price = (int)orderDetail.Product.SellingPrice;
                item.Height = 6;
                item.Length = 8;
                item.Width = 8;
                item.Weight = (int)   (orderDetail.Product.Weight *1000);
                listItem.Add(item);
            }
            orderRequest.Items = listItem;
            request.Content = new StringContent(JsonConvert.SerializeObject(orderRequest), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var responseReturn = new Response<CreateOrderResponse>();
            if(!response.IsSuccessStatusCode) {
                responseReturn = new Response<CreateOrderResponse>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = $"Lỗi từ API GHN: {content}",
                    Data = null
                };
                return responseReturn;
            }
        

            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<CreateOrderResponse>>(content);

            responseReturn = new Response<CreateOrderResponse>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
            return responseReturn;
        }
        #endregion

        #region Calculate Fee
        public async Task<Response<FeeDataResponse>> CalculateShippingFee(FeeRequest requestOrder)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/v2/shipping-order/fee");
            AddHeaders(request);

            requestOrder = new FeeRequest
            {
                FromDistrictId = (int.Parse(_returnDistrictId)),
                FromWardCode = _returnWardCode,
                ServiceTypeId = 2,
                ToDistrictId = requestOrder.ToDistrictId,
                ToWardCode = requestOrder.ToWardCode,
                Weight = requestOrder.Weight,
            };

            var jsonRequest = JsonConvert.SerializeObject(requestOrder, Formatting.Indented);
            Console.WriteLine("Request sent: " + jsonRequest);

            request.Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response received: " + content);

            var responseReturn = new Response<FeeDataResponse>();
            if (!response.IsSuccessStatusCode)
            {
                responseReturn = new Response<FeeDataResponse>
                {
                    StatusCode = (int)response.StatusCode,
                    Message = $"Lỗi từ API GHN: {content}",
                    Data = null
                };
                return responseReturn;
            }

            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<FeeDataResponse>>(content);

            responseReturn = new Response<FeeDataResponse>
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
            return responseReturn;
        }

        #endregion


        #region GET SERVICE TYPE
        public async Task<Response<List<GHNServiceType>>> GetServiceTypes(int fromDistrictId, int toDistrictId)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/v2/shipping-order/available-services");

            AddHeaders(request);

            var requestBody = new
            {
                shop_id = int.Parse(_shopId), // ID của shop
                from_district = fromDistrictId,
                to_district = toDistrictId
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var responseReturn = new Response<List<GHNServiceType>>();

            if (!response.IsSuccessStatusCode)
            {
                return new Response<List<GHNServiceType>>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = $"Lỗi từ API GHN: {content}",
                    Data = null
                };
            }

            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<List<GHNServiceType>>>(content);

            responseReturn = new Response<List<GHNServiceType>>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
            return responseReturn;

        }
        #endregion

        #region Calculate ExpectedDateDelivery

        public async Task<Response<GHNExpectedDateDelivery>> GetExpectedDateDelivery(int fromDistrictId,string fromWardCode, int toDistrictId, string toWardCode)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, $"{_apiUrl}/v2/shipping-order/leadtime");

            AddHeaders(request);

            var requestBody = new
            {
                service_id = 5,
                from_district_id = fromDistrictId,
                to_district_id = toDistrictId,
                from_ward_code = fromWardCode,
                to_ward_code = toWardCode
            };

            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            var responseReturn = new Response<GHNExpectedDateDelivery>();

            if (!response.IsSuccessStatusCode)
            {
                return new Response<GHNExpectedDateDelivery>()
                {
                    StatusCode = (int)response.StatusCode,
                    Message = $"Lỗi từ API GHN: {content}",
                    Data = null
                };
            }

            var jsonResponse = JsonConvert.DeserializeObject<GHNResponse<GHNExpectedDateDelivery>>(content);
            GHNExpectedDateDelivery result = jsonResponse.Data;
            
            jsonResponse.Data.ExpectedDeliveryDate = result.GetLeadTimeDate();
            jsonResponse.Data.OrderedDate = DateTime.Now;

            responseReturn = new Response<GHNExpectedDateDelivery>()
            {
                StatusCode = (int)response.StatusCode,
                Data = jsonResponse.Data
            };
            return responseReturn;

        }

        #endregion

    }
}
