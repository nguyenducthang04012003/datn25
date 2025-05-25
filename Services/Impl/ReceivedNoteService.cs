using AutoMapper;

using PharmaDistiPro.DTO.ReceivedNotes;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Helper.Enums;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class ReceivedNoteService : IReceivedNoteService
    {
        private readonly IReceivedNoteRepository _receivedNoteRepository;
        private readonly IPurchaseOrderRepository _purchaseOrderRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        public ReceivedNoteService(IReceivedNoteRepository receivedNoteRepository, 
            IHttpContextAccessor httpContextAccessor, 
            IMapper mapper, 
            IPurchaseOrderRepository purchaseOrderRepository)
        {
            _receivedNoteRepository = receivedNoteRepository;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _purchaseOrderRepository = purchaseOrderRepository;

        }

        public async Task<Response<ReceivedNoteDto>> CreateReceiveNote(ReceivedNoteRequest ReceiveNote)
        {
            var purchaseOrder = await _purchaseOrderRepository.GetPoById(ReceiveNote.PurchaseOrderId.Value);
            var userId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);
            try
            {

            
            if (purchaseOrder == null)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = "Không tìm thấy đơn đặt hàng!",
                    StatusCode = 404
                };
            }


            ReceivedNote receiveNoteNew = new ReceivedNote
            {
                CreatedDate = DateTime.Now,
                PurchaseOrderId = ReceiveNote.PurchaseOrderId,
                CreatedBy = userId,
                Status = ReceiveNote.Status
            };

            receiveNoteNew = await _receivedNoteRepository.CreateReceivedNote(receiveNoteNew);

            if (receiveNoteNew == null)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = "Lỗi khi tạo phiếu nhập!",
                    StatusCode = 500
                };
            }


            // Duyệt qua tất cả các ReceivedNoteDetail trong phiếu nhập mới
            foreach (var item in ReceiveNote.ReceivedNoteDetail)
            {
                ProductLot productLotUpdate = await _receivedNoteRepository.GetProductLotById(item.ProductLotId);
                if (productLotUpdate == null)
                {
                    return new Response<ReceivedNoteDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy lô hàng!",
                        StatusCode = 404
                    };
                }

                // Cập nhật số lượng của lô hàng
                productLotUpdate.Quantity += item.ActualReceived;
                productLotUpdate = await _receivedNoteRepository.UpdateProductLot(productLotUpdate);

                if (productLotUpdate == null)
                {
                    return new Response<ReceivedNoteDto>
                    {
                        Success = false,
                        Message = "Lỗi khi cập nhật lô hàng!",
                        StatusCode = 500
                    };
                }

                // Lưu chi tiết phiếu nhập
                ReceivedNoteDetail receivedNoteDetail = new ReceivedNoteDetail
                {
                    ReceiveNoteId = receiveNoteNew.ReceiveNoteId,
                    ProductLotId = item.ProductLotId,
                    ActualReceived = item.ActualReceived
                };

                receivedNoteDetail = await _receivedNoteRepository.CreateReceivedNoteDetail(receivedNoteDetail);
                if (receivedNoteDetail == null)
                {
                    return new Response<ReceivedNoteDto>
                    {
                        Success = false,
                        Message = "Lỗi khi tạo chi tiết phiếu nhập!",
                        StatusCode = 500
                    };
                }

            }
            //
            bool isShortage = false;

            // Duyệt qua tất cả các ReceivedNoteDetail đã có trong hệ thống (tất cả các phiếu nhập đã được lưu)
            var allReceivedNoteDetails = await _receivedNoteRepository.GetReceivedNoteDetailsByPurchaseOrderId(ReceiveNote.PurchaseOrderId);

            // Kiểm tra xem đơn đặt hàng đã hoàn thành hay còn thiếu hàng
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
                    isShortage = true;
                }
            }



            // Cập nhật trạng thái của PurchaseOrder
            purchaseOrder.Status = isShortage ? (int)PurchaseOrderStatus.THIEU_HANG : (int)PurchaseOrderStatus.HOAN_THANH;
            await _purchaseOrderRepository.UpdateAsync(purchaseOrder);
            await _purchaseOrderRepository.SaveAsync();

            return new Response<ReceivedNoteDto>
            {
                Success = true,
                Message = isShortage ? "Tạo phiếu nhập thành công nhưng còn thiếu hàng!" : "Tạo phiếu nhập thành công!",
                Data = _mapper.Map<ReceivedNoteDto>(receiveNoteNew),
                StatusCode = 200
            };
            }
            catch (Exception ex)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = ex.Message,
                    StatusCode = 500
                };
            }
        }




        public async Task<Response<ReceivedNoteResponse>> GetReceiveNoteById(int id)
        {
            if (id <= 0)
            {
                return new Response<ReceivedNoteResponse>
                {
                    Success = false,
                    Message = "Id không hợp lệ!",
                    StatusCode = 400
                };
            }
            var receiveNote = await _receivedNoteRepository.GetReceivedNoteById(id);
            var receivedNoteDetails = await _receivedNoteRepository.GetReceivedNoteDetailByReceivedNoteId(id);
            if (receiveNote == null || receivedNoteDetails == null)
            {
                return new Response<ReceivedNoteResponse>
                {
                    Success = false,
                    Message = "Không tìm thấy phiếu nhập!",
                    StatusCode = 404
                };
            }
            ReceivedNoteResponse response = new ReceivedNoteResponse
            {
                ReceiveNoteId = receiveNote.ReceiveNoteId,
                ReceivedNoteCode = receiveNote.ReceiveNotesCode,
                PurchaseOrderCode = receiveNote.PurchaseOrder.PurchaseOrderCode,
                CreatedDate = receiveNote.CreatedDate ?? DateTime.MinValue, // Handle nullable DateTime with a default value
                CreatedBy = receiveNote.CreatedBy?.ToString() ?? string.Empty, // Fix for CS0029 and CS8601
                Status = receiveNote.Status?.ToString() ?? string.Empty, // Convert nullable int to string and handle null
                PurchaseOrderId = receiveNote.PurchaseOrderId ?? 0, // Handle nullable int with a default value
                ReceivedNoteDetails = receivedNoteDetails.Select(x => new ReceiveNoteDetailResponse
                {
                    ProductLotId = x.ProductLotId ?? 0, // Handle nullable int with a default value
                    ActualReceived = x.ActualReceived ?? 0, // Handle nullable int with a default value
                    ProductName = x.ProductLot.Product.ProductName,
                    LotCode = x.ProductLot.Lot.LotCode,
                    ProductCode = x.ProductLot.Product.ProductCode,
                    Unit = x.ProductLot.Product.Unit,
                    UnitPrice = (int)x.ProductLot.SupplyPrice                   

                }).ToList() ?? new List<ReceiveNoteDetailResponse>() // Handle null collection
            };
            return new Response<ReceivedNoteResponse>
            {
                Success = true,
                Message = "Lấy phiếu nhập thành công!",
                Data = response,
                StatusCode = 200
            };
        }

        public async Task<Response<List<ReceivedNoteDto>>> GetReceiveNoteList()
        {
            var receiveNoteList = await _receivedNoteRepository.GetReceivedNoteList();
            if (receiveNoteList == null)
            {
                return new Response<List<ReceivedNoteDto>>
                {
                    Success = false,
                    Message = "Không tìm thấy danh sách phiếu nhập!",
                    StatusCode = 404
                };
            }
            if(receiveNoteList.Count <= 0)
            {
                return new Response<List<ReceivedNoteDto>>
                {
                    Success = false,
                    Message = "Danh sách phiếu nhập rỗng!",
                    StatusCode = 404
                };
            }
            return new Response<List<ReceivedNoteDto>>
            {
                Success = true,
                Message = "Lấy danh sách phiếu nhập thành công!",
                Data = _mapper.Map<List<ReceivedNoteDto>>(receiveNoteList),
                StatusCode = 200
            };
        }

        public async Task<Response<ReceivedNoteDto>> CancelReceiveNote(int? ReceivedNoteId)
        {
            if (ReceivedNoteId <= 0)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = "Id không hợp lệ!",
                    StatusCode = 400
                };
            }
            ReceivedNote receiveNoteUpdate = await _receivedNoteRepository.GetReceivedNoteById(ReceivedNoteId);
            if (receiveNoteUpdate == null)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = "Không tìm thấy phiếu nhập!",
                    StatusCode = 404
                };
            }
            
            

            receiveNoteUpdate.Status = -1;

            receiveNoteUpdate = await _receivedNoteRepository.UpdateReceivedNote(receiveNoteUpdate);
            if (receiveNoteUpdate == null)
            {
                return new Response<ReceivedNoteDto>
                {
                    Success = false,
                    Message = "Lỗi khi cập nhật phiếu nhập!",
                    StatusCode = 500
                };
            }
            return new Response<ReceivedNoteDto>
            {
                Success = true,
                Message = "Cập nhật phiếu nhập thành công!",
                Data = _mapper.Map<ReceivedNoteDto>(receiveNoteUpdate),
                StatusCode = 200
            };

        }
    }
}
