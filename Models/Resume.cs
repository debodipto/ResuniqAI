using System.ComponentModel.DataAnnotations;

namespace ResuniqAI.Models
{
    public class Resume
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Summary { get; set; }

        public string Education { get; set; }

        public string Experience { get; set; }

        public string Skills { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}