namespace SOS_API.Models
{
    public class ValidationResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}