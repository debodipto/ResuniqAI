namespace ResuniqAI.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string Plan { get; set; } = "Free";
        public DateTime StartDate { get; set; } = DateTime.Now;
        public DateTime ExpireDate { get; set; }
        public bool IsActive { get; set; }
    }
}