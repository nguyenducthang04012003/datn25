using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.PurchaseOrders;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseOrdersController : ControllerBase
    {
        private readonly IPurchaseOrderService _purchaseOrderService;
        public PurchaseOrdersController(IPurchaseOrderService purchaseOrderService)
        {
            _purchaseOrderService = purchaseOrderService;
        }

        #region purchase order
        [HttpPost("CreatePurchaseOrders")]
  
        public async Task<IActionResult> CreatePurchaseOrder([FromBody]PurchaseOrdersRequestDto purchaseOrdersRequestDto)
        {
            var response = await _purchaseOrderService.CreatePurchaseOrder(purchaseOrdersRequestDto);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpPut("UpdatePurchaseOrderStatus/{poId}/{status}")]
        public async Task<IActionResult> UpdatePurchaseOrderStatus(int poId, int status)
        {
            var response = await _purchaseOrderService.UpdatePurchaseOrderStatus(poId, status);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("GetPurchaseOrdersList")]
  
        public async Task<IActionResult> GetPurchaseOrdersList([FromQuery]int[] status, DateTime? dateFrom, DateTime? dateTo)
        {
            var response = await _purchaseOrderService.GetPurchaseOrdersList(status, dateFrom, dateTo);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("GetPurchaseOrdersRevenueList")]
        public async Task<IActionResult> GetPurchaseOrdersRevenueList(DateTime? dateFrom, DateTime? dateTo)
        {
            var response = await _purchaseOrderService.GetPurchaseOrdersRevenueList(dateFrom, dateTo);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        [HttpGet("GetTopSupplierList")]
        public async Task<IActionResult> GetTopSupplierList(int topSupplier)
        {
            var response = await _purchaseOrderService.GetTopSupplierList(topSupplier);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }

        #endregion

        #region purchase order detail
        [HttpGet("GetPurchaseOrderDetailByPoId/{poId}")]
        public async Task<IActionResult> GetPurchaseOrderDetailByPoId(int poId)
        {
            var response = await _purchaseOrderService.GetPurchaseOrderDetailByPoId(poId);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
        #endregion

        #region check po status
        [HttpGet("CheckReceivedStockStatus/{purchaseOrderId}")]
        public async Task<IActionResult> CheckReceivedStockStatus(int purchaseOrderId)
        {
            var response = await _purchaseOrderService.CheckReceivedStockStatus(purchaseOrderId);
            if (response.Success)
                return Ok(response);

            return BadRequest(response);
        }
        #endregion
    }
}
