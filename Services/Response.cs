namespace PharmaDistiPro.Services
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new List<string>();
        public int StatusCode { get; set; } = 200; // Default to OK
    }
}
