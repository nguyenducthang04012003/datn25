using AutoMapper;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PharmaDistiPro.Common.Enums;
using PharmaDistiPro.DTO.IssueNote;
using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.DTO.OrdersDetails;
using PharmaDistiPro.DTO.Products;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Helper;
using PharmaDistiPro.Models;
using PharmaDistiPro.Repositories.Impl;
using PharmaDistiPro.Repositories.Interface;
using PharmaDistiPro.Services.Interface;
using System.Collections;
using System.Linq;

namespace PharmaDistiPro.Services.Impl
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrdersDetailRepository _ordersDetailRepository;

        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository,
           IIssueNoteRepository issuteNoteRepository,
           IProductRepository productRepository,
           IOrdersDetailRepository ordersDetailRepository,
           IMapper mapper, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _ordersDetailRepository = ordersDetailRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        #region order
        // get order cua customer
        public async Task<Response<IEnumerable<OrderDto>>> GetOrderByCustomerId()
        {
            Response<IEnumerable<OrderDto>> response = new Response<IEnumerable<OrderDto>>();
            var userId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);
            try
            {
                var orders = await _orderRepository.GetByConditionAsync(o => o.CustomerId == userId, includes: new string[] { "ConfirmedByNavigation", "Customer" });
                if (orders == null)
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    orders = orders.OrderByDescending(o => o.OrderId).ToList();
                    response.Data = _mapper.Map<IEnumerable<OrderDto>>(orders);
                    response.Success = true;
                    return response;
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
            }
            return response;

        }

        // get all order trong he thong
        public async Task<Response<IEnumerable<OrderDto>>> GetAllOrders(int[] status, DateTime? dateCreatedFrom, DateTime? dateCreatedTo)
        {
            var response = new Response<IEnumerable<OrderDto>>();
            try
            {
                IEnumerable<Order> ordersList = await _orderRepository.GetByConditionAsync(
                 x => (!status.Any() || status.Contains(x.Status ?? 1)) &&
                (!dateCreatedFrom.HasValue || x.CreatedDate >= dateCreatedFrom.Value) &&
                (!dateCreatedTo.HasValue || x.CreatedDate <= dateCreatedTo.Value),
                includes: new string[] { "ConfirmedByNavigation", "Customer" });

                if (!ordersList.Any())
                {
                    return new Response<IEnumerable<OrderDto>>
                    {
                        Success = false,
                        Message = "Không có dữ liệu"

                    };
                }

                response.Data = _mapper.Map<IEnumerable<OrderDto>>(ordersList);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<OrderDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // check out  sau khi add to cart
        public async Task<Response<OrderDto>> CheckOut(OrderRequestDto orderRequestDto)
        {
            var response = new Response<OrderDto>();
            try
            {
             

                #region Create Order
                var order = _mapper.Map<Models.Order>(orderRequestDto);
                double? totalAmount = 0;

                //var productList = productList

                if (orderRequestDto.OrdersDetails != null)
                {
                    foreach (var orderDetails in orderRequestDto.OrdersDetails)
                    {
                        var product = _productRepository.GetById(orderDetails.ProductId);
                        totalAmount += (product.SellingPrice * orderDetails.Quantity + product.SellingPrice * orderDetails.Quantity * product.Vat / 100);
                    }
                }

                if (totalAmount != orderRequestDto.TotalAmount)
                {
                    response.Success = false;
                    response.Message = "Tính sai giá trị đơn hàng";
                    return response;
                }
                order.CreatedDate = DateTime.Now;
                order.StockReleaseDate = null;
                order.ConfirmedBy = null;
                order.Status = (int)Common.Enums.OrderStatus.DANG_CHO_THANH_TOAN;
                order.UpdatedStatusDate = DateTime.Now;
                order.CustomerId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);



                await _orderRepository.InsertOrderAsync(order);
                await _orderRepository.SaveAsync();
                #endregion

                #region Create Order Details


                // Thực hiện mapping
                List<OrdersDetail> ordersDetails = _mapper.Map<List<OrdersDetail>>(orderRequestDto.OrdersDetails);
                ordersDetails.ForEach(x => x.OrderId = order.OrderId);

                await _ordersDetailRepository.AddOrdersDetails(ordersDetails);
                await _ordersDetailRepository.SaveAsync(); // Save toàn bộ OrderDetails
                #endregion

                response.Success = true;
                response.Data = _mapper.Map<OrderDto>(order);
                return response;
            }
            catch (Exception ex)
            {
                return new Response<OrderDto>
                {
                    Success = false,
                    Message = $"Lỗi khi tạo đơn hàng: {ex.Message}"
                };
            }
        }

        //Update trang thai order
        public async Task<Response<OrderDto>> UpdateOrderStatus(int orderId, int status)
        {
            var response = new Response<OrderDto>();
            try
            {
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy đơn hàng";
                    return response;
                }
                order.Status = status;
                order.UpdatedStatusDate = DateTime.Now;

                await _orderRepository.UpdateAsync(order);
                await _orderRepository.SaveAsync();
                response.Success = true;
                response.Data = _mapper.Map<OrderDto>(order);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        //list order can confirm
        public async Task<Response<IEnumerable<OrderDto>>> GetOrderNeedConfirm()
        {
            var response = new Response<IEnumerable<OrderDto>>();
            try
            {
                var orders = await _orderRepository.GetByConditionAsync(o => o.Status == (int)Common.Enums.OrderStatus.DANG_CHO_XAC_NHAN,
                    includes: new string[] { "ConfirmedByNavigation", "Customer" });
            
                if (!orders.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                     orders = orders.OrderByDescending(o => o.OrderId).ToList();
                    response.Data = _mapper.Map<IEnumerable<OrderDto>>(orders);
                    response.Success = true;
                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }

        }

        //confirm order
        public async Task<Response<OrderDto>> ConfirmOrder(int orderId)
        {
            var response = new Response<OrderDto>();
            try
            {
                #region Update Order Status
                var order = await _orderRepository.GetByIdAsync(orderId);
                if (order == null)
                {
                    response.Success = false;
                    response.Message = "Không tìm thấy đơn hàng";
                    return response;
                }
                order.Status = (int)Common.Enums.OrderStatus.XAC_NHAN;
                order.UpdatedStatusDate = DateTime.Now;
                order.ConfirmedBy = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);
                //assign cho warehouse manager
                var warehouseManager = await _userRepository.GetWarehouseManagerToConfirm();
                order.AssignTo = warehouseManager.UserId;

                await _orderRepository.UpdateAsync(order); // Lưu trạng thái mới của đơn hàng trước
                await _orderRepository.SaveAsync();
                #endregion


                response.Success = true;
                response.Data = _mapper.Map<OrderDto>(order);
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.InnerException?.Message ?? ex.Message;
                return response;
            }
        }

        // get order revenue
        public async Task<Response<IEnumerable<OrderDto>>> GetOrdersRevenueList(DateTime? dateCreatedFrom, DateTime? dateCreatedTo)
        {
            var response = new Response<IEnumerable<OrderDto>>();
            try
            {
                IEnumerable<Order> ordersList = await _orderRepository.GetByConditionAsync(
                 x => x.Status == (int)Common.Enums.OrderStatus.HOAN_THANH &&
                (!dateCreatedFrom.HasValue || x.CreatedDate >= dateCreatedFrom.Value) &&
                (!dateCreatedTo.HasValue || x.CreatedDate <= dateCreatedTo.Value),
                includes: new string[] { "ConfirmedByNavigation", "Customer" });

               

                if (!ordersList.Any())
                {
                    return new Response<IEnumerable<OrderDto>>
                    {
                        Success = false,
                        Message = "Không có dữ liệu"

                    };
                }
                ordersList = ordersList.OrderByDescending(o => o.OrderId);
                response.Data = _mapper.Map<IEnumerable<OrderDto>>(ordersList);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<OrderDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        // get order cua warehouse de tao issue note
        public async Task<Response<IEnumerable<OrderDto>>> GetOrderToCreateIssueNoteList()
        {
            var response = new Response<IEnumerable<OrderDto>>();
            var userId = UserHelper.GetUserIdLogin(_httpContextAccessor.HttpContext);
            try
            {
                var orders = await _orderRepository.GetByConditionAsync(o => o.AssignTo == userId && o.Status == (int)Common.Enums.OrderStatus.XAC_NHAN,
                    includes: new string[] { "ConfirmedByNavigation", "Customer" });

                
                if (!orders.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    orders = orders.OrderByDescending(o => o.OrderId).ToList();
                    response.Data = _mapper.Map<IEnumerable<OrderDto>>(orders);
                    response.Success = true;
                    return response;
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }

        #endregion

        #region order details
        // get all order details to order by product quantity
        public async Task<Response<IEnumerable<OrdersDetailDto>>> GetAllOrderDetails(DateTime? dateFrom, DateTime? dateTo, int? topProduct)
        {
            var response = new Response<IEnumerable<OrdersDetailDto>>();
            try
            {
                
                IEnumerable<OrdersDetail> ordersDetails = await _ordersDetailRepository.GetByConditionAsync(x =>
                x.Order.Status == (int)Common.Enums.OrderStatus.HOAN_THANH &&
                    (!dateFrom.HasValue || x.Order.CreatedDate >= dateFrom.Value) &&
                (!dateTo.HasValue || x.Order.CreatedDate <= dateTo.Value),
                    includes: new string[] { "Product", "Order" });

                // Nhóm theo ProductId, tính tổng Quantity và sắp xếp giảm dần
                var groupedOrders = ordersDetails
                    .GroupBy(o => o.ProductId)
                    .Select(g => new
                    {
                        ProductId = g.Key,
                        TotalQuantity = g.Sum(o => o.Quantity),
                        Orders = g.ToList()
                    })
                    .OrderByDescending(g => g.TotalQuantity)
                    .ToList(); // Chuyển thành danh sách để tránh lỗi khi gọi Take()

                // Nếu topProduct có giá trị, chỉ lấy topProduct sản phẩm
                if (topProduct.HasValue)
                {
                    groupedOrders = groupedOrders.Take(topProduct.Value).ToList(); // Đảm bảo kiểu dữ liệu nhất quán
                }

                 var resultOrders = groupedOrders
                    .Select(g => new OrdersDetailDto
                    {
                        ProductId = g.ProductId,
                        TotalQuantity = g.TotalQuantity,
                        Product = _mapper.Map<ProductOrderDto>(g.Orders.First().Product) // Lấy thông tin sản phẩm từ danh sách orders
                    }).ToList();

                if (!resultOrders.Any())
                {
                    return new Response<IEnumerable<OrdersDetailDto>>
                    {
                        Success = false,
                        Message = "Không có dữ liệu"
                    };
                }

                response.Data = _mapper.Map<IEnumerable<OrdersDetailDto>>(resultOrders);
                response.Success = true;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<OrdersDetailDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        //get order detail theo orderId
        public async Task<Response<IEnumerable<OrdersDetailDto>>> GetOrderDetailByOrderId(int orderId)
        {
            var response = new Response<IEnumerable<OrdersDetailDto>>();
            try
            {
                var ordersDetails = await _ordersDetailRepository.GetByConditionAsync(o => o.OrderId == orderId, includes: new string[] { "Product" });

                if (!ordersDetails.Any())
                {
                    response.Success = false;
                    response.Message = "Không có dữ liệu";
                }
                else
                {
                    ordersDetails = ordersDetails.OrderByDescending(o => o.OrderDetailId);

                var orderDto = _mapper.Map<IEnumerable<OrdersDetailDto>>(ordersDetails);
                    response.Data = orderDto;
                    response.Success = true;
                }
                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                return response;
            }
        }
        #endregion

        #region Customer revenue
        //List top customer revenue
        public async Task<Response<IEnumerable<OrderDto>>> GetTopCustomerRevenue(int? topCustomer)
        {
            var response = new Response<IEnumerable<OrderDto>>();

            try
            {
                var orders = await _orderRepository.GetByConditionAsync(
                    x => x.Status == (int)Common.Enums.OrderStatus.HOAN_THANH,
                    includes: new string[] { "Customer", "ConfirmedByNavigation" });

                if (!orders.Any())
                {
                    return new Response<IEnumerable<OrderDto>>
                    {
                        Success = false,
                        Message = "Không có dữ liệu"
                    };
                }

                var groupedOrders = orders
                    .GroupBy(x => x.CustomerId)
                    .Select(g => new
                    {
                        CustomerId = g.Key,
                        TotalRevenue = g.Sum(x => x.TotalAmount),
                        Customer = g.ToList()
                    })
                    .OrderByDescending(g => g.TotalRevenue)
                    .Take(topCustomer ?? 5) // Lấy số lượng khách hàng theo tham số hoặc mặc định là 5
                    .ToList();


                // Chuyển đổi sang DTO (có thể bạn muốn map danh sách Orders chứ không phải cả nhóm)
                var resultOrders = groupedOrders
                   .Select(g => new OrderDto
                   {
                       CustomerId = g.CustomerId,
                       TotalRevenue = g.TotalRevenue,
                       Customer = _mapper.Map<UserDTO>(g.Customer.First().Customer) // Lấy thông tin sản phẩm từ danh sách orders
                   }).ToList();
                response.Success = true;
                response.Data = resultOrders;
            }
            catch (Exception ex)
            {
                return new Response<IEnumerable<OrderDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }

            return response;
        }

        #endregion




    }
}
