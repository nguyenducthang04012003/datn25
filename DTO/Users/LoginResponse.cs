namespace PharmaDistiPro.DTO.Users
{
    public class LoginResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        public string RoleName { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserAvatar { get; set; }
        public bool VerifyFlag { get; set; }
        public bool? IdentifyFlg { get; set; }
    }
}
