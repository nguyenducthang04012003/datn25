namespace PharmaDistiPro.DTO.GHN
{
    public class GHNResponse<T>
    {
        public int Code { get; set; } 
        public T Data { get; set; }
    }
}
