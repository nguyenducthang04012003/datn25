using PharmaDistiPro.Common.Helper;
using PharmaDistiPro.DTO.Carts;
using PharmaDistiPro.Services.Interface;

namespace PharmaDistiPro.Services.Impl
{
    public class CartService : ICartService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Response<CartModelDto>> AddToCart(CartModelDto cartModelDto)
        {
           
            var response = new Response<CartModelDto>();
            try
            {
                // goi list cart tu cookie
                var cartList =  CookieHelper.GetCartFromCookie(_httpContextAccessor.HttpContext);
                //check xem item i da co trong cart chua
                var itemExist = cartList.FirstOrDefault(c => c.ProductId == cartModelDto.ProductId);

                if(cartModelDto.Quantity <= 0)
                {
                    response.Message = "Số lượng không hợp lệ";
                    response.Success = false;
                    return response;
                }
                if(itemExist == null)
                {
                    cartList.Add(cartModelDto);
                    response.Message = "Thêm vào giỏ hàng thành công";
                    response.Success = true;                  
                } else
                {
                    itemExist.Quantity += cartModelDto.Quantity;
                    response.Message = "Cập nhật giỏ hàng thành công";
                    response.Success = true;
                }
                response.Data = cartModelDto;
                CookieHelper.SetCartCookie(_httpContextAccessor.HttpContext, cartList);
                return response;
            }
            catch(Exception ex)
            {
                return new Response<CartModelDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
            
        }

        public async Task<Response<IEnumerable<CartModelDto>>> GetCartList()
        {
            var response = new Response<IEnumerable<CartModelDto>>();
            try
            {
                var cartList = CookieHelper.GetCartFromCookie(_httpContextAccessor.HttpContext);
                if(cartList.Count == 0)
                {
                    response.Message = "Không có dữ liệu";
                }
                response.Data = cartList;
                response.Success = true;
                return response;
            }
            catch(Exception ex)
            {
                return new Response<IEnumerable<CartModelDto>>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<CartModelDto>> RemoveFromCart(int productId)
        {
            var response = new Response<CartModelDto>();
            try
            {
                var cartList = CookieHelper.GetCartFromCookie(_httpContextAccessor.HttpContext);
                // kiem tra xem item co trong cart khong de remove
                var itemExist = cartList.FirstOrDefault(c => c.ProductId == productId);
                if(itemExist == null)
                {
                    response.Message = "Không tìm thấy sản phẩm trong giỏ hàng";
                    response.Success = false;
                } else
                {
                    cartList.Remove(itemExist);
                    response.Message = "Xóa sản phẩm khỏi giỏ hàng thành công";
                    response.Success = true;
                }
                CookieHelper.SetCartCookie(_httpContextAccessor.HttpContext, cartList);
                return response;
            }
            catch(Exception ex)
            {
                return new Response<CartModelDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Response<CartModelDto>> UpdateCart(int productId, int quantity)
        {
            var response = new Response<CartModelDto>();
            try
            {
                var cartList = CookieHelper.GetCartFromCookie(_httpContextAccessor.HttpContext);
                
                var itemExist = cartList.FirstOrDefault(c => c.ProductId == productId);
                if (itemExist == null)
                {
                    response.Message = "Không tìm thấy sản phẩm trong giỏ hàng";
                    response.Success = false;
                }
                else
                {
                    if(quantity == 0) {
                        cartList.Remove(itemExist);
                        response.Message = "Xóa sản phẩm khỏi giỏ hàng thành công";
                        response.Success = true;
                    }else
                    {
                        itemExist.Quantity = quantity;
                        response.Message = "Cập nhật số lượng thành công";
                        response.Success = true;
                    }
                }
                CookieHelper.SetCartCookie(_httpContextAccessor.HttpContext, cartList);
                response.Data = itemExist;
                return response;
            }
            catch (Exception ex)
            {
                return new Response<CartModelDto>
                {
                    Success = false,
                    Message = ex.Message
                };
            }
        }
    }
}
