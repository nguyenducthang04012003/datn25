namespace PharmaDistiPro.DTO.Units
{
    public class UnitInputRequest
    {
        public int Id { get; set; }
        public string? UnitsName { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
