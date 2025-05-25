using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace PharmaDistiPro.Helper
{
    public class UserHelper
    {
        public static int GetUserIdLogin(HttpContext httpContext)
        {
            if (httpContext?.User?.Identity?.IsAuthenticated != true)
            {
                return 0; 
            }

            var userIdClaim = httpContext.User.Claims
                .FirstOrDefault(c => c.Type == "UserId")?.Value;

            return int.TryParse(userIdClaim, out int userId) ? userId : 1;
        }
    }
}
