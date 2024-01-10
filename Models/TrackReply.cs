namespace sweep_email_api.Models
{
    public class TrackReply
    {
        public string TrackId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}
