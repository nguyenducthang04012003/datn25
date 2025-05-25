using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class IventoryActivity
    {
        public int LogId { get; set; }
        public string? EvenType { get; set; }
        public string? Message { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? RequestUrl { get; set; }
        public string? StackTrace { get; set; }
        public string? Ipaddress { get; set; }

        public virtual User? User { get; set; }
    }
}
