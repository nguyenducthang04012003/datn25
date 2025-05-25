using Newtonsoft.Json;
using PharmaDistiPro.DTO.Carts;

namespace PharmaDistiPro.Common.Helper
{
    public class CookieHelper
    {
        public static void SetCartCookie(HttpContext httpContext, List<CartModelDto> cartItems)
        {
            string cartJson = JsonConvert.SerializeObject(cartItems);
            CookieOptions options = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30), // Cookie hết hạn sau 30 ngày
                IsEssential = true
            };
            httpContext.Response.Cookies.Append("Cart", cartJson, options);
        }
        public static List<CartModelDto> GetCartFromCookie(HttpContext httpContext)
        {
            var cartJson = httpContext.Request.Cookies["Cart"];
            return cartJson != null ? JsonConvert.DeserializeObject<List<CartModelDto>>(cartJson) : new List<CartModelDto>();
        }
    }
}
