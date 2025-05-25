using Microsoft.AspNetCore.Mvc;
using PharmaDistiPro.DTO.Users;
using PharmaDistiPro.Models;


namespace PharmaDistiPro.Services.Interface
{
    public interface IUserService
    {

      //  Task<LoginResponse> Login(LoginRequest loginModel);


        #region User and Customer Management
        Task<Response<IEnumerable<UserDTO>>> GetUserList();
        Task<Response<IEnumerable<UserDTO>>> GetCustomerList();

        Task<Response<UserDTO>> ActivateDeactivateUser(int userId, bool update);

        Task<Response<UserDTO>> CreateNewUserOrCustomer(UserInputRequest user);
        Task<Response<UserDTO>> GetUserById(int userId);
        Task<Response<UserDTO>> UpdateUser(UserInputRequest user);
        #endregion

        #region Authentication
        Task<Services.Response<LoginResponse>> Logout([FromBody] string refreshToken);
        Task<Services.Response<LoginResponse>> RefreshToken(TokenModel tokenModel);
        Task<User> getUserByEmail(string email);
        Task<User> UpdateUser(User user);
        Task<Services.Response<LoginResponse>> Login(LoginRequest loginModel);
        Task<Services.Response<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
        #endregion

    }
}
