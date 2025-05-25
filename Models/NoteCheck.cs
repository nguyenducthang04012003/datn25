using System;
using System.Collections.Generic;

namespace PharmaDistiPro.Models
{
    public partial class NoteCheck
    {
        public NoteCheck()
        {
            NoteCheckDetails = new HashSet<NoteCheckDetail>();
        }

        public int NoteCheckId { get; set; }
        public string? NoteCheckCode { get; set; }
        public int? StorageRoomId { get; set; }
        public string? ReasonCheck { get; set; }
        public string? Result { get; set; }
        public bool? Status { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual StorageRoom? StorageRoom { get; set; }
        public virtual ICollection<NoteCheckDetail> NoteCheckDetails { get; set; }
    }
}
