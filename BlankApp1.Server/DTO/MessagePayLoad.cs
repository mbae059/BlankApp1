namespace BlankApp1.Server.DTO
{
    public class MessagePayLoad
    {
        public string User { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
