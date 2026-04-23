namespace ResuniqAI.Models
{
    public class Payment
    {
        public int Id { get; set; }
        public string UserEmail { get; set; } = "";
        public string TransactionId { get; set; } = "";
        public bool IsApproved { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}