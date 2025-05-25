using PharmaDistiPro.DTO.Orders;
using PharmaDistiPro.Models;
using System.Text.Json.Serialization;

namespace PharmaDistiPro.DTO.Users
{
    public class UserDTO
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public byte[]? Password { get; set; }
        public int? Age { get; set; }
        public string? Avatar { get; set; }
        public string? Address { get; set; }
        public int? RoleId { get; set; }
        public string? EmployeeCode { get; set; }
        public string? TaxCode { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? RoleName { get; set; }

    }
}
