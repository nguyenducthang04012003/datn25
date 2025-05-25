using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.DTO.OrdersDetails;
using PharmaDistiPro.Services;
using PharmaDistiPro.Services.Impl;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IGHNService _ghnService;
        public OrderController(IOrderService orderService, IGHNService ghnService)
        {
            _orderService = orderService;
            _ghnService = ghnService;
        }

        #region Orders

        //Api get order by customer id
        [HttpGet("GetOrderByCustomerId")]
        public async Task<IActionResult> GetOrderByCustomerId()
        {
            var response = await _orderService.GetOrderByCustomerId();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api get all order trong he thong
        [HttpGet("GetAllOrders")]
        public async Task<IActionResult> GetAllOrders([FromQuery] int[] status, DateTime? createdDateFrom, DateTime? createdDateTo)
        {
            var response = await _orderService.GetAllOrders(status, createdDateFrom, createdDateTo);

            if (!response.Success) return BadRequest(new { response.Message });

            return Ok(response);
        }


        //api get list order can confirm
        [HttpGet("GetOrderNeedConfirm")]
        public async Task<IActionResult> GetOrderNeedConfirm()
        {
            var response = await _orderService.GetOrderNeedConfirm();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api create order
        [HttpPost("CheckOut")]
        public async Task<IActionResult> CheckOut([FromBody]OrderRequestDto orderRequestDto)
        {
            var response = await _orderService.CheckOut(orderRequestDto);

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }
            return Ok(response);
        }

        // api update order status
        [HttpPut("UpdateOrderStatus/{orderId}/{status}")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int status)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, status);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        // api get order to create issue note
        [HttpGet("GetOrderToCreateIssueNoteList")]
        public async Task<IActionResult> GetOrderToCreateIssueNoteList()
        {
            var response = await _orderService.GetOrderToCreateIssueNoteList();
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        //api confirm order
        [HttpPut("ConfirmOrder/{orderId}")]
        public async Task<IActionResult> ConfirmOrder(int orderId)
        {
            var response = await _orderService.ConfirmOrder(orderId);
            if (!response.Success)
            {
                return BadRequest(new { response.Message });
            }
            var result = await _ghnService.CreateOrder(orderId);
            return Ok(response);
        }

        // api get orders revenue list
        [HttpGet("GetOrdersRevenueList")]
        public async Task<IActionResult> GetOrdersRevenueList(DateTime? dateCreatedFrom, DateTime? dateCreatedTo)
        {
            var response = await _orderService.GetOrdersRevenueList(dateCreatedFrom, dateCreatedTo);

            if (!response.Success) return BadRequest(new { response.Message });

            return Ok(response);
        }

        #endregion

        #region order details

        // api get orders detail order by quantity product
        [HttpGet("GetAllOrderDetails")]
        public async Task<IActionResult> GetAllOrderDetails(DateTime? dateFrom, DateTime? dateTo, int? topProduct)
        {
            var response = await _orderService.GetAllOrderDetails(dateFrom, dateTo, topProduct);

            if (!response.Success) return BadRequest(new { response.Message });

            return Ok(response);
        }


        // api get order details cua order
        [HttpGet("GetOrdersDetailByOrderId/{orderId}")]
        public async Task<IActionResult> GetOrdersDetailByOrderId(int orderId)
        {
            var response = await _orderService.GetOrderDetailByOrderId(orderId);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }

        #endregion
        #region Customer revenue
        //List top customer revenue

        [HttpGet("GetTopCustomerRevenue")]
        public async Task<IActionResult> GetTopCustomerRevenue(int? topCustomer)
        {
            var response = await _orderService.GetTopCustomerRevenue(topCustomer);
            if (!response.Success)
            {
                return NotFound(new { response.Message });
            }
            return Ok(response);
        }
        #endregion



    }
}
