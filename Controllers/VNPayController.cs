using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text;
using VNPAY.NET.Enums;
using VNPAY.NET.Models;
using VNPAY.NET.Utilities;
using VNPAY.NET;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VNPayController : ControllerBase
    {


        private readonly HttpClient _httpClient;

        private readonly IVnpay _vnpay;
        private readonly IConfiguration _configuration;

        public VNPayController(IVnpay vnpay, IConfiguration configuration, HttpClient httpClient)
        {
            _vnpay = vnpay;
            _configuration = configuration;
            var tmnCode = configuration["Vnpay:TmnCode"];
            var hashSecret = configuration["Vnpay:HashSecret"];
            var baseUrl = configuration["Vnpay:BaseUrl"];
            var callbackUrl = configuration["Vnpay:CallbackUrl"];
            _vnpay.Initialize(_configuration["Vnpay:TmnCode"], _configuration["Vnpay:HashSecret"], _configuration["Vnpay:BaseUrl"], _configuration["Vnpay:CallbackUrl"]);
            _httpClient = httpClient;
        }
        #region create payment
        [HttpGet("CreatePaymentUrl")]
        public IActionResult CreatePaymentUrl(double moneyToPay, string description, int orderId)
        {
            try
            { 
               var ipAddress = "103.166.183.58"; // Lấy địa chỉ IP của thiết bị thực hiện giao dịch

            var request = new PaymentRequest
                {
                    PaymentId = orderId,
                    Money = moneyToPay,
                    Description = description,
                    IpAddress = ipAddress,
                    BankCode = BankCode.ANY, // Tùy chọn. Mặc định là tất cả phương thức giao dịch
                    CreatedDate = DateTime.Now, // Tùy chọn. Mặc định là thời điểm hiện tại
                    Currency = Currency.VND, // Tùy chọn. Mặc định là VND (Việt Nam đồng)
                    Language = DisplayLanguage.Vietnamese // Tùy chọn. Mặc định là tiếng Việt
                };

                var paymentUrl = _vnpay.GetPaymentUrl(request);

                return Created(paymentUrl, paymentUrl);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message + "Vào exception");
            }
        }
        #endregion
        #region stuff
        //[HttpGet("IpnAction")]
        //public IActionResult IpnAction()
        //{
        //    if (Request.QueryString.HasValue)
        //    {
        //        try
        //        {
        //            var paymentResult = _vnpay.GetPaymentResult(Request.Query);
        //            if (paymentResult.IsSuccess)
        //            {
        //                // Thực hiện hành động nếu thanh toán thành công tại đây. Ví dụ: Cập nhật trạng thái đơn hàng trong cơ sở dữ liệu.
        //                Console.WriteLine("XU LI SUCCESS BACKEND");
        //                return Ok();
        //            }

        //            // Thực hiện hành động nếu thanh toán thất bại tại đây. Ví dụ: Hủy đơn hàng.
        //            Console.WriteLine("HUY THANH TOAN");
        //            return BadRequest("Thanh toán thất bại");
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(ex.Message);
        //        }
        //    }

        //    return NotFound("Không tìm thấy thông tin thanh toán.");
        //}
        #endregion
        #region call back
        [HttpGet("Callback")]
        public IActionResult Callback()
        {
            if (Request.QueryString.HasValue)
            {
                try
                {
                    var paymentResult = _vnpay.GetPaymentResult(Request.Query);
                    var transactionNo = Request.Query["vnp_TransactionNo"];
                    var txnRef = Request.Query["vnp_TxnRef"];
                    var resultDescription = $"{paymentResult.PaymentResponse.Description}. {paymentResult.TransactionStatus.Description}.";

                    if (paymentResult.IsSuccess)
                    {
                        

                          var Url = $"http://localhost:5173/payment/success?orderId={txnRef}";
                    
                        Console.WriteLine($"✅ Callback thành công:");
                        Console.WriteLine($"➡️  Mã giao dịch (TxnRef): {txnRef}");
                        Console.WriteLine($"➡️  Mã thanh toán (TransactionNo): {transactionNo}");
                        return Redirect(Url);
                    }
                    Console.WriteLine("FAILED");



                    var UrlFail = $"http://localhost:5173/payment/failed?orderId={txnRef}";
                   

                    return Redirect($"http://localhost:5173/payment/failed?orderId={txnRef}");
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);

                }
            }

            return NotFound("Không tìm thấy thông tin thanh toán.");
        }
        #endregion
        #region refund
        [HttpPost("Refund")]
        public async Task<IActionResult> Refund([FromBody] RefundRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Validate configuration
                var refundUrl = _configuration["Vnpay:RefundUrl"];
                var tmnCode = _configuration["Vnpay:TmnCode"];
                var hashSecret = _configuration["Vnpay:HashSecret"];

                if (string.IsNullOrEmpty(refundUrl) || string.IsNullOrEmpty(tmnCode) || string.IsNullOrEmpty(hashSecret))
                {
                    Console.WriteLine("Configuration error: RefundUrl, TmnCode, or HashSecret is missing.");
                    return StatusCode(500, new { Error = "Server configuration error: Missing VNPay settings" });
                }

                // Log configuration for debugging
                Console.WriteLine($"RefundUrl: {refundUrl}");
                Console.WriteLine($"TmnCode: {tmnCode}");
                Console.WriteLine($"HashSecret: {(string.IsNullOrEmpty(hashSecret) ? "null" : "****")}");

                // Build request data
                var requestData = new Dictionary<string, string>
        {
            { "vnp_RequestId", DateTime.UtcNow.Ticks.ToString() },
            { "vnp_Version", "2.1.0" },
            { "vnp_Command", "refund" },
            { "vnp_TmnCode", tmnCode },
            { "vnp_TransactionType", "02" }, // Full refund; adjust if partial refund is needed
            { "vnp_TxnRef", request.OrderId.ToString() },
            { "vnp_Amount", (request.Amount * 100).ToString() }, // Convert to VNPay format (VND * 100)
            { "vnp_OrderInfo", $"Refund for order {request.OrderId}" },
            { "vnp_TransactionNo", request.TransactionNo },
            { "vnp_CreateBy", request.User },
            { "vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss") },
            { "vnp_IpAddr", NetworkHelper.GetIpAddress(HttpContext) }
        };

                // Generate secure hash
                var sortedData = requestData.OrderBy(x => x.Key);
                var signData = string.Join("&", sortedData.Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
                var secureHash = HmacSHA512(hashSecret, signData);
                requestData.Add("vnp_SecureHash", secureHash);

                // Log request data for debugging
                Console.WriteLine("RequestData: {0}", string.Join(", ", requestData.Select(kv => $"{kv.Key}={kv.Value}")));
                Console.WriteLine($"Refund signature data: {signData}");
                Console.WriteLine($"Refund secure hash: {secureHash}");

                // Send HTTP request
                using var content = new FormUrlEncodedContent(requestData);
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.PostAsync(refundUrl, content);
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP request failed: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return StatusCode(500, new { Error = "Failed to connect to VNPay", Details = ex.Message });
                }
                catch (TaskCanceledException ex)
                {
                    Console.WriteLine($"HTTP request timed out: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return StatusCode(504, new { Error = "VNPay request timed out", Details = ex.Message });
                }

                // Check response
                Console.WriteLine($"Response StatusCode: {(response != null ? response.StatusCode.ToString() : "null")}");

                if (response == null)
                {
                    Console.WriteLine("Response is null");
                    return StatusCode(500, new { Error = "No response received from VNPay" });
                }

                // Read response content
                string responseString;
                try
                {
                    responseString = await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading response: {ex.Message}\nStackTrace: {ex.StackTrace}");
                    return StatusCode(500, new { Error = "Failed to read VNPay response", Details = ex.Message });
                }

                // Log response
                Console.WriteLine($"VNPay Response: Status: {response.StatusCode}, Body: {responseString}");

                // Handle non-success status codes
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"VNPay refund failed. Status: {response.StatusCode}, Response: {responseString}");
                    return StatusCode((int)response.StatusCode, new { Error = "VNPay refund request failed", Details = responseString });
                }

                // Success
                return Ok(responseString);
            }
            catch (Exception ex)
            {
                // Log detailed exception
                Console.WriteLine($"Refund error: {ex.Message}\nStackTrace: {ex.StackTrace}\nInnerException: {ex.InnerException?.Message}");
                return StatusCode(500, new { Error = "An error occurred while processing refund", Details = ex.Message });
            }
        }

        private static string HmacSHA512(string key, string inputData)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(inputData))
                throw new ArgumentNullException("Key or input data cannot be null or empty");

            using var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLowerInvariant();
        }
        #endregion
    }
    public class RefundRequestDto
    {
        [Required]
        [Range(1, long.MaxValue)]
        public long OrderId { get; set; }

        [Required]
        [Range(0.01, long.MaxValue)]
        public long Amount { get; set; }

        [Required]
        [MaxLength(50)]
        public string TransactionNo { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string User { get; set; } = string.Empty;
    }

}

