using AutoMapper;
using CloudinaryDotNet;
using MailKit.Search;
using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.DTO.ProductShortage;
using PharmaDistiPro.DTO.PurchaseOrders;
using PharmaDistiPro.DTO.PurchaseOrdersDetails;
using PharmaDistiPro.DTO.Suppliers;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IPurchaseOrdersDetailRepository _purchaseOrdersDetailRepository;
        private readonly IMapper _mapper;

        public PurchaseOrderService(IPurchaseOrderRepository purchaseOrderRepository, IPurchaseOrdersDetailRepository purchaseOrdersDetailRepository, IMapper mapper)
        {
            _purchaseOrderRepository = purchaseOrderRepository;
            _purchaseOrdersDetailRepository = purchaseOrdersDetailRepository;
            _mapper = mapper;
        }

        #region purchase orders
        public async Task<Response<PurchaseOrdersDto>> CreatePurchaseOrder(PurchaseOrdersRequestDto purchaseOrdersRequestDto)
        {
            var response = new Response<PurchaseOrdersDto>();
            try
            {
 

                // Tạo PurchaseOrder
                var purchaseOrder = _mapper.Map<PurchaseOrder>(purchaseOrdersRequestDto);
                purchaseOrder.UpdatedStatusDate = DateTime.Now;
                purchaseOrder.Status = (int)PurchaseOrderStatus.CHO_NHAP_HANG;
                purchaseOrder.CreateDate = DateTime.Now;

                decimal? totalAmount = 0;

                foreach (var poDetails in purchaseOrdersRequestDto.PurchaseOrdersDetails)
                {
                    totalAmount += (poDetails.SupplyPrice * poDetails.Quantity);
                }

                if (totalAmount != decimal.Parse(purchaseOrdersRequestDto.TotalAmount.ToString()))
                {
                    response.Success = false;
                    response.Message = "Tổng tiền đơn hàng sai";
                    return response;
                }
                await _purchaseOrderRepository.InsertPurchaseOrderAsync(purchaseOrder);
                await _purchaseOrderRepository.SaveAsync();


                // Chuẩn bị danh sách PurchaseOrderDetails
                var purchaseOrderDetails = purchaseOrdersRequestDto.PurchaseOrdersDetails
                    .Select(item =>
                    {
                        var detail = _mapper.Map<PurchaseOrdersDetail>(item);
                        detail.PurchaseOrderId = purchaseOrder.PurchaseOrderId;
                        return detail;
                    }).ToList();
                    

                // Thêm danh sách chi tiết đơn hàng cùng lúc
                await _purchaseOrdersDetailRepository.InsertRangeAsync(purchaseOrderDetails);
                await _purchaseOrdersDetailRepository.SaveAsync();

                response.Data = _mapper.Map<PurchaseOrdersDto>(purchaseOrder);
                response.Success = true;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
            }

            return response;
        }

        public async Task<Response<IEnumerable<PurchaseOrdersDto>>> GetPurchaseOrdersList(int[] status, DateTime? dateFrom, DateTime? dateTo)
        {
           var response = new Response<IEnumerable<PurchaseOrdersDto>>();
            try
            {
                var purchaseOrders = await _purchaseOrderRepository.GetByConditionAsync(x => (!status.Any() || status.Contains(x.Status ?? 1)) &&
                (!dateFrom.HasValue || x.CreateDate >= dateFrom) &&
                (!dateTo.HasValue || x.CreateDate <= dateTo),
                includes: new string[] { "CreatedByNavigation", "Supplier" });

               

                if (!purchaseOrders.Any())
                {
                    response.Message = "Không có dữ liệu";
                    response.Success = false;
                    return response;
                }
                purchaseOrders = purchaseOrders.OrderByDescending(x => x.PurchaseOrderId);
                response.Data = _mapper.Map<IEnumerable<PurchaseOrdersDto>>(purchaseOrders);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<IEnumerable<PurchaseOrdersDto>>> GetPurchaseOrdersRevenueList(DateTime? dateFrom, DateTime? dateTo)
        {
            var response = new Response<IEnumerable<PurchaseOrdersDto>>();
            try
            {
                var purchaseOrders = await _purchaseOrderRepository.GetByConditionAsync(x => x.Status == (int)PurchaseOrderStatus.HOAN_THANH &&
                    (!dateFrom.HasValue || x.CreateDate >= dateFrom) &&
                    (!dateTo.HasValue || x.CreateDate <= dateTo),
                    includes: new string[] { "CreatedByNavigation", "Supplier" });
                if (!purchaseOrders.Any())
                {
                    response.Message = "Không có dữ liệu";
                    response.Success = false;
                    return response;
                }
                purchaseOrders = purchaseOrders.OrderByDescending(x => x.PurchaseOrderId);
                response.Data = _mapper.Map<IEnumerable<PurchaseOrdersDto>>(purchaseOrders);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
                return response;
            }
        }

        public async Task<Response<IEnumerable<PurchaseOrdersDto>>> GetTopSupplierList(int? topSupplier)
        {
            var response = new Response<IEnumerable<PurchaseOrdersDto>>();
            try
            {
                var purchaseOrders = await _purchaseOrderRepository.GetByConditionAsync(x => x.Status == (int)PurchaseOrderStatus.HOAN_THANH,
                    includes: new string[] { "CreatedByNavigation", "Supplier" });

                     if (!purchaseOrders.Any())
                {
                    return new Response<IEnumerable<PurchaseOrdersDto>>
                    {
                        Success = false,
                        Message = "Không có dữ liệu"
                    };
                }
                var groupedOrders = purchaseOrders.GroupBy(x=> x.SupplierId)
                    .Select(x=> new
                    {
                        SupplierId = x.Key,
                        AmountPaid = x.Sum(x => x.TotalAmount),
                        Supplier = x.ToList()

                    })
                    .ToList();
            /*    var groupedOrders = purchaseOrders.GroupBy(x => x.SupplierId)
                    .Select(x => new {                  
                        SupplierId = x.Key,                    
                        AmountPaid = x.Sum(y => y.TotalAmount),
                        Supplier = x.ToList()
                    })
                    .OrderByDescending(x => x.AmountPaid)
                    .Take(topSupplier ?? 5);*/

                var resultOrders = groupedOrders.Select(x => new PurchaseOrdersDto
                {
                   SupplierId = x.SupplierId,
                    Supplier = _mapper.Map<SupplierDTO>(x.Supplier.FirstOrDefault().Supplier),
                    AmountPaid = x.AmountPaid

                }).ToList();

                response.Data = _mapper.Map<IEnumerable<PurchaseOrdersDto>>(resultOrders);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
                return response;
            }
        }



        public async Task<Response<PurchaseOrdersDto>> UpdatePurchaseOrderStatus(int poId, int status)
        {
            var response = new Response<PurchaseOrdersDto>();
            try
            {
                var purchaseOrder = await _purchaseOrderRepository.GetByIdAsync(poId);
                if (purchaseOrder == null)
                {
                    response.Message = "Không có dữ liệu";
                    response.Success = false;
                    return response;
                }

                purchaseOrder.Status = status;
                purchaseOrder.UpdatedStatusDate = DateTime.Now;

                await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
                await _purchaseOrderRepository.SaveAsync();

                response.Data = _mapper.Map<PurchaseOrdersDto>(purchaseOrder);
                response.Success = true;
                return response;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
                return response;
            }
        }
        #endregion

        #region purchase order detail
        public async Task<Response<IEnumerable<PurchaseOrdersDetailDto>>> GetPurchaseOrderDetailByPoId(int poId)
        {
            var response = new Response<IEnumerable<PurchaseOrdersDetailDto>>();
            try
            {
                var purchaseOrderDetails = await _purchaseOrdersDetailRepository.GetByConditionAsync(x => x.PurchaseOrderId == poId,
                    includes: new string[] { "Product" });
                    
                if (!purchaseOrderDetails.Any())
                {
                    response.Message = "Không có dữ liệu";
                    response.Success = false;
                    return response;
                }

                var result = _mapper.Map<IEnumerable<PurchaseOrdersDetailDto>>(purchaseOrderDetails);
                response.Data = result;
                response.Success = true;
                return response;
            }catch(Exception ex)
            {
                response.Message = ex.Message;
                response.Success = false;
                return response;
            }
        }

        #endregion

        #region check po status
        public async Task<Response<List<ProductShortage>>> CheckReceivedStockStatus(int purchaseOrderId)
        {
            try
            {

           
            var purchaseOrder = await _purchaseOrderRepository.GetPoById(purchaseOrderId);

            if (purchaseOrder == null)
            {
                return new Response<List<ProductShortage>>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn hàng",
                    StatusCode = 404
                };
            }
            
            var shortages = new List<ProductShortage>();

            // Tổng hợp số lượng đặt hàng theo sản phẩm
            var orderedProducts = purchaseOrder.PurchaseOrdersDetails
                .GroupBy(pod => pod.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(pod => pod.Quantity ?? 0));

            // Tổng hợp số lượng thực nhận theo sản phẩm từ tất cả phiếu nhập kho
            var receivedProducts = purchaseOrder.ReceivedNotes
                .SelectMany(rn => rn.ReceivedNoteDetails)
                .Where(rnd => rnd.ProductLot != null) // Đảm bảo ProductLot tồn tại
                .GroupBy(rnd => rnd.ProductLot.ProductId)
                .ToDictionary(g => g.Key, g => g.Sum(rnd => rnd.ActualReceived ?? 0));

            // Kiểm tra từng sản phẩm xem có thiếu hàng hay không
            foreach (var kvp in orderedProducts)
            {
                int productId = (int)kvp.Key;
                int orderedQty = kvp.Value;
                int receivedQty = receivedProducts.ContainsKey(productId) ? receivedProducts[productId] : 0;
                int shortage = orderedQty - receivedQty;

                if (shortage > 0)
                {
                    shortages.Add(new ProductShortage
                    {
                        ProductId = productId,
                        ProductName = purchaseOrder.PurchaseOrdersDetails
                                        .First(pod => pod.ProductId == productId).Product.ProductName,
                        OrderedQuantity = orderedQty,
                        ReceivedQuantity = receivedQty,
                        ShortageQuantity = shortage
                    });
                }
            }

            return new Response<List<ProductShortage>>
            {
               Success = true,
               Data = shortages,
               StatusCode = 200
            };
            }
            catch (Exception ex)
            {
                return new Response<List<ProductShortage>>
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }
        #endregion
    }
}
