using PharmaDistiPro.DTO.IssueNote;
using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.DTO.OrdersDetails;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Models;

namespace PharmaDistiPro.Services.Interface
{
    public interface IOrderService
    {
        #region orders
        //List all orders
        Task<Response<IEnumerable<OrderDto>>> GetAllOrders(int[] status, DateTime? dateCreatedFrom, DateTime? dateCreatedTo);

        //Create order checkout
        Task<Response<OrderDto>> CheckOut(OrderRequestDto orderRequestDto);

        //Update order status
        Task<Response<OrderDto>> UpdateOrderStatus(int orderId, int status);

        //confirm order status
        Task<Response<OrderDto>> ConfirmOrder(int orderId);
        //List order dang can confirm
        Task<Response<IEnumerable<OrderDto>>> GetOrderNeedConfirm();

        //List all orders revenue
        Task<Response<IEnumerable<OrderDto>>> GetOrdersRevenueList(DateTime? dateCreatedFrom, DateTime? dateCreatedTo);

        //List order cua customer
        Task<Response<IEnumerable<OrderDto>>> GetOrderByCustomerId();

        // List order cua warehouse để create issue note
        Task<Response<IEnumerable<OrderDto>>> GetOrderToCreateIssueNoteList();

        #endregion

        #region order details

        //List ra orderDetail theo orderId
        Task<Response<IEnumerable<OrdersDetailDto>>> GetOrderDetailByOrderId(int orderId);

        // list full order details
        Task<Response<IEnumerable<OrdersDetailDto>>> GetAllOrderDetails(DateTime? dateFrom, DateTime? dateTo, int? topProduct);
        #endregion

        #region Customer revenue
        //List top customer revenue
        Task<Response<IEnumerable<OrderDto>>> GetTopCustomerRevenue(int? topCustomer);
        #endregion

    }
}
