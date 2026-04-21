using System.Net.Http.Json;

namespace ResuniqAI.Services
{
    public class AIService
    {
        private readonly HttpClient _http;

        public AIService(HttpClient http)
        {
            _http = http;
        }

        // 🧠 Resume Summary AI
        public async Task<string> GenerateSummary(string skills, string experience)
        {
            return $"Highly skilled professional with expertise in {skills}. Experienced in {experience}. Strong problem-solving ability and teamwork mindset.";
        }

        // 📊 ATS Score Checker
        public int CalculateATSScore(string skills, string experience, string education)
        {
            int score = 50;

            if (!string.IsNullOrEmpty(skills)) score += 20;
            if (!string.IsNullOrEmpty(experience)) score += 20;
            if (!string.IsNullOrEmpty(education)) score += 10;

            return score > 100 ? 100 : score;
        }

        // ✉️ Cover Letter Generator
        public string GenerateCoverLetter(string name, string role, string skills)
        {
            return $"Dear Hiring Manager,\n\nMy name is {name}. I am applying for the position of {role}. I have strong skills in {skills} and I am confident I can contribute to your company’s success.\n\nSincerely,\n{name}";
        }
    }
}