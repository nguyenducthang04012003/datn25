using System.ComponentModel.DataAnnotations;

namespace PharmaDistiPro.DTO.Users
{
    public class ResetPasswordRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string OTP { get; set; } = string.Empty;

        [Required, MinLength(6, ErrorMessage = "Please enter at least 6 characters")]
        public string Password { get; set; } = string.Empty;

        [Required, Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

    }
}
