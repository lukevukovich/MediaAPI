namespace MediaAPI.Models
{
    public class ProxyResult<T>
    {
        public bool Success { get; set; }
        public T? Value { get; set; }
        public string? ErrorMessage { get; set; }
    }
}