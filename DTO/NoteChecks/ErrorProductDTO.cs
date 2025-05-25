namespace PharmaDistiPro.DTO.NoteChecks
{
    public class ErrorProductDTO
    {
        public string ProductName { get; set; }
        public int ErrorQuantity { get; set; }
        public string LotId { get; set; }
        public string NoteCheckCode { get; set; }

        public int NoteCheckDetailId { get; set; }
        public string ErrorStatus { get; set; } // Trạng thái của hàng hỏng: "Đang xử lý" hoặc "Đã hủy"
    }

    public class CancelErrorProductRequest
    {
        public string NoteCheckCode { get; set; }
        public string LotId { get; set; }
    }
}
