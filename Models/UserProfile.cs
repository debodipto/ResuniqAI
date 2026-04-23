namespace ResuniqAI.Models
{
    public class UserProfile
    {
        public int Id { get; set; }
        public string UserId { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Headline { get; set; } = "";
        public string Location { get; set; } = "";
        public string Phone { get; set; } = "";
        public string LinkedIn { get; set; } = "";
        public string Github { get; set; } = "";
        public string Portfolio { get; set; } = "";
        public string Bio { get; set; } = "";
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}
