using AutoMapper;
using PharmaDistiPro.DTO.IssueNote;
using PharmaDistiPro.DTO.IssueNoteDetails;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using System.Linq;

namespace PharmaDistiPro.Services.Impl
{
    public class IssueNoteService : IIssueNoteService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrdersDetailRepository _ordersDetailRepository;
        private readonly IProductLotRepository _productLotRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStorageRoomRepository _storageRoomRepository;
        private readonly IIssueNoteDetailsRepository _issueNoteDetailsRepository;
        private readonly IIssueNoteRepository _issueNoteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IssueNoteService(IOrderRepository orderRepository,
            IIssueNoteRepository issuteNoteRepository,
            IIssueNoteDetailsRepository issueNoteDetailsRepository,
            IOrdersDetailRepository ordersDetailRepository,
            IProductLotRepository productLotRepository, IMapper mapper, IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,IProductRepository productRepository
, IStorageRoomRepository storageRoomRepository)
        {
            _issueNoteRepository = issuteNoteRepository;
            _issueNoteDetailsRepository = issueNoteDetailsRepository;
            _orderRepository = orderRepository;
            _ordersDetailRepository = ordersDetailRepository;
            _productLotRepository = productLotRepository;
            _productRepository = productRepository;
            _storageRoomRepository = storageRoomRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _storageRoomRepository = storageRoomRepository;
        }

        #region IssueNote
        public async Task<Response<IssueNoteDto>> CancelIssueNote(int issueNoteId)
        {
            var response = new Response<IssueNoteDto>();
            try
            {
                // 1. Kiểm tra phiếu xuất kho có tồn tại không
                var issueNote = await _issueNoteRepository.GetByIdAsync(issueNoteId);
                if (issueNote == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy phiếu xuất kho";
                    return response;
                }

                // 2. Cập nhật trạng thái phiếu xuất kho thành HỦY
                issueNote.Status = (int)Common.Enums.IssueNotesStatus.HUY;
                issueNote.UpdatedStatusDate = DateTime.Now;

                // 3. Lấy danh sách chi tiết phiếu xuất kho
                var issueNoteDetails = await _issueNoteDetailsRepository.GetByConditionAsync(x => x.IssueNoteId == issueNoteId);

                if (!issueNoteDetails.Any())
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy chi tiết phiếu xuất kho";
                    return response;
                }

                // 4. Lấy danh sách ProductLotId từ IssueNoteDetail
                var productLotIds = issueNoteDetails
                    .Where(x => x.ProductLotId.HasValue)
                    .Select(x => x.ProductLotId.Value)
                    .ToList();

                // 5. Lấy danh sách ProductLot từ productLotIds
                var productLots = await _productLotRepository.GetByConditionAsync(x => productLotIds.Contains(x.ProductLotId));

                // 6. Cập nhật lại số lượng trong kho cho từng ProductLot
                foreach (var productLot in productLots)
                {
                    // Lọc các issueNoteDetail tương ứng với productLot hiện tại
                    var relatedIssueDetails = issueNoteDetails.Where(x => x.ProductLotId == productLot.ProductLotId);

                    foreach (var issueNoteDetail in relatedIssueDetails)
                    {
                        if (issueNoteDetail.Quantity.HasValue)
                        {
                            productLot.Quantity += issueNoteDetail.Quantity.Value;
                        }
                    }

                    await _productLotRepository.UpdateAsync(productLot);
                }

                // 7. Lưu thay đổi
                await _issueNoteRepository.SaveAsync();

                // 8. Trả về kết quả thành công
                response.Success = true;
                response.Data = _mapper.Map<IssueNoteDto>(issueNote);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<Response<IssueNoteDto>> CreateIssueNote(int orderId)
        {
            var response = new Response<IssueNoteDto>();

            try
            {
                #region 1. Kiểm tra đơn hàng và cập nhật trạng thái
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    return new Response<IssueNoteDto>
                    {
                        Success = false,
                        Message = "Không tìm thấy đơn hàng"
                    };
                }

                order.Status = (int)Common.Enums.OrderStatus.VAN_CHUYEN;
                order.UpdatedStatusDate = DateTime.Now;
                await _orderRepository.UpdateAsync(order);
                #endregion

                #region 2. Tạo IssueNote
                var issueNote = new IssueNote
                {
                    OrderId = orderId,
                    CreatedDate = DateTime.Now,
                    UpdatedStatusDate = DateTime.Now,
                    Status = (int)Common.Enums.IssueNotesStatus.DA_XUAT,
                    CustomerId = order.CustomerId,
                    TotalAmount = order.TotalAmount,
                    CreatedBy = order.AssignTo
                };

                await _issueNoteRepository.CreateIssueNote(issueNote);
                await _orderRepository.SaveAsync(); // Để issueNote.Id có giá trị
                #endregion

                #region 3. Xử lý chi tiết đơn hàng & xuất kho
                var orderDetails = await _ordersDetailRepository.GetByConditionAsync(x => x.OrderId == orderId);
                var productIds = orderDetails
                    .Where(x => x.ProductId.HasValue)
                    .Select(x => x.ProductId.Value)
                    .Distinct()
                    .ToList();

                var productLots = (await _productLotRepository.GetProductLotsByProductIds(productIds)).ToList();
                if (!productLots.Any())
                {
                    return new Response<IssueNoteDto>
                    {
                        Success = false,
                        Message = "Không có hàng trong kho"
                    };
                }

                // Lấy danh sách sản phẩm theo Ids
                var products = await _productRepository.GetByIdsAsync(productIds); // đảm bảo bạn có hàm này trong repo
                var productDict = products.ToDictionary(p => p.ProductId, p => p);

                var issueNoteDetailsList = new List<IssueNoteDetail>();

                foreach (var detail in orderDetails)
                {
                    int remainingQty = detail.Quantity ?? 0;
                    int productId = detail.ProductId ?? 0;

                    var lots = productLots
                        .Where(l => l.ProductId == productId && l.Quantity > 0)
                        .OrderBy(l => l.ExpiredDate);

                    foreach (var lot in lots)
                    {
                        if (remainingQty <= 0) break;

                        int takeQty = Math.Min(remainingQty, lot.Quantity ?? 0);
                        remainingQty -= takeQty;
                        lot.Quantity -= takeQty;

                        issueNoteDetailsList.Add(new IssueNoteDetail
                        {
                            IssueNoteId = issueNote.IssueNoteId,
                            ProductLotId = lot.ProductLotId,
                            Quantity = takeQty
                        });

                        await _productLotRepository.UpdateAsync(lot);

                        // Trả lại thể tích kho
                        if (productDict.TryGetValue(productId, out var product) && product.VolumePerUnit.HasValue)
                        {
                            double volume = Math.Round(takeQty * ((double)product.VolumePerUnit.Value / 1000000), 2);

                            if (lot.StorageRoomId.HasValue)
                            {
                                var storageRoom = await _storageRoomRepository.GetByIdAsync(lot.StorageRoomId.Value);
                                if (storageRoom != null)
                                {
                                    storageRoom.RemainingRoomVolume = Math.Round((storageRoom.RemainingRoomVolume ?? 0) + volume, 2);
                                    await _storageRoomRepository.UpdateAsync(storageRoom);
                                }
                            }
                        }
                    }

                    if (remainingQty > 0)
                    {
                        return new Response<IssueNoteDto>
                        {
                            Success = false,
                            Message = $"Không đủ hàng để xuất kho cho sản phẩm ID: {productId}"
                        };
                    }
                }
                #endregion

                #region 4. Lưu chi tiết và hoàn tất
                await _issueNoteDetailsRepository.InsertRangeAsync(issueNoteDetailsList);
                await _orderRepository.SaveAsync(); // lưu thay đổi sản phẩm, kho
                #endregion

                response.Success = true;
                response.Data = _mapper.Map<IssueNoteDto>(issueNote);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }
        public async Task<Response<IEnumerable<IssueNoteDto>>> GetIssueNoteByWarehouseId(int[]? status)
        {
            var response = new Response<IEnumerable<IssueNoteDto>>();
            try
            {
                var userId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);
                // Ensure status is not null before checking Contains
                var issueNotes = await _issueNoteRepository.GetByConditionAsync(
                    x => x.CreatedBy == userId && (!status.Any() || status.Contains(x.Status ?? 2)));

                if (!issueNotes.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                    return response;
                }
                issueNotes = issueNotes.OrderByDescending(x => x.IssueNoteId).ToList();
                response.Success = true;
                response.Data = _mapper.Map<IEnumerable<IssueNoteDto>>(issueNotes);
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
            }
            return response;
        }


        public async Task<Response<IEnumerable<IssueNoteDto>>> GetIssueNoteList()
        {
            var response = new Response<IEnumerable<IssueNoteDto>>();
            try
            {
                var issueNotes = await _issueNoteRepository.GetAllAsync();
                if (!issueNotes.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                issueNotes = issueNotes.OrderByDescending(x => x.IssueNoteId).ToList();
                response.Success = true;
                response.Data = _mapper.Map<IEnumerable<IssueNoteDto>>(issueNotes);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<Response<IssueNoteDto>> UpdateIssueNoteStatus(int issueNoteId, int status)
        {
            var response = new Response<IssueNoteDto>();
            try
            {
                var issueNote = await _issueNoteRepository.GetSingleByConditionAsync(x => x.IssueNoteId == issueNoteId);
                if (issueNote == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy phiếu xuất kho";
                    return response;
                } 
                // update issue status
                issueNote.Status = status;
                issueNote.UpdatedStatusDate = DateTime.Now;

                await _issueNoteRepository.UpdateAsync(issueNote);
                await _issueNoteRepository.SaveAsync();

                response.Success = true;
                response.Data = _mapper.Map<IssueNoteDto>(issueNote);
                return response;

            }catch(Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        #endregion

        #region IssueNoteDetail

        public async Task<Response<IEnumerable<IssueNoteDetailDto>>> GetIssueNoteDetailByIssueNoteId(int issueNoteId)
        {
            var response = new Response<IEnumerable<IssueNoteDetailDto>>();
            try
            {
                var issueNoteDetails = await _issueNoteDetailsRepository.GetByConditionAsync(x => x.IssueNoteId == issueNoteId,
     includes: new string[] { "ProductLot", "ProductLot.Product", "ProductLot.StorageRoom" });


                if (!issueNoteDetails.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    response.Success = true;
                    response.Data = _mapper.Map<IEnumerable<IssueNoteDetailDto>>(issueNoteDetails);  
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        public async Task<Response<IEnumerable<IssueNoteDetailDto>>> GetIssueNoteDetailsList()
        {
            var response = new Response<IEnumerable<IssueNoteDetailDto>>();
            try
            {
                var issueNoteDetails = await _issueNoteDetailsRepository
                    .GetByConditionAsync(
                        x => true,
                       includes: new string[] { "ProductLot", "ProductLot.Product", "ProductLot.StorageRoom" }
                    );
                if (!issueNoteDetails.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    response.Success = true;
                    response.Data = _mapper.Map<IEnumerable<IssueNoteDetailDto>>(issueNoteDetails);
                }
                return response;

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }
        #endregion
    }
}
