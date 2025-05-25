using PharmaDistiPro.DTO.Carts;

namespace PharmaDistiPro.Services.Interface
{
    public interface ICartService
    {
        public Task<Response<IEnumerable<CartModelDto>>> GetCartList();

        public Task<Response<CartModelDto>> AddToCart(CartModelDto cartModelDto);

        public Task<Response<CartModelDto>> UpdateCart(int productId, int quantity);

        public Task<Response<CartModelDto>> RemoveFromCart(int productId);
    }
}
