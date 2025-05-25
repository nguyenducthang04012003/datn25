namespace PharmaDistiPro.DTO.ProductShortage
{
    public class ProductShortage
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int OrderedQuantity { get; set; }
        public int ReceivedQuantity { get; set; }
        public int ShortageQuantity { get; set; }
    }
}
