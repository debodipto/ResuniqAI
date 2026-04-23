using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ResuniqAI.Services
{
    public class AIService
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AIService> _logger;

        public AIService(HttpClient http, IConfiguration configuration, ILogger<AIService> logger)
        {
            _http = http;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateSummary(string skills, string experience)
        {
            return await GenerateSummaryFromPrompt("", skills, experience, "", "");
        }

        public async Task<string> GenerateSummaryFromPrompt(string prompt, string skills, string experience, string role, string company)
        {
            var focus = NormalizePrompt(prompt, "professional resume summary");
            var roleText = string.IsNullOrWhiteSpace(role) ? "professional" : role;
            var companyText = string.IsNullOrWhiteSpace(company) ? "high-impact teams" : company;
            var skillText = string.IsNullOrWhiteSpace(skills) ? "communication, execution and problem solving" : skills;
            var experienceText = string.IsNullOrWhiteSpace(experience) ? "delivering reliable results across responsibilities" : experience;

            var fallback = $"Results-driven {roleText} with experience in {experienceText}. Brings strength in {skillText} and adapts quickly to business priorities. Focused on {focus}, contributing measurable value, and supporting strong delivery for {companyText}.";

            var userPrompt = $"""
Write a professional resume summary in 3 to 4 sentences.
User instruction: {focus}
Target role: {roleText}
Target company or team: {companyText}
Skills: {skillText}
Experience context: {experienceText}

Make it ATS-friendly, concise, achievement-oriented, and ready to paste into a resume.
""";

            return await GenerateTextWithFallbackAsync(
                "You write polished, ATS-friendly resume summaries for job applicants. Return only the final summary text without headings or markdown.",
                userPrompt,
                fallback);
        }

        public async Task<string> GenerateExperienceBullets(string company, string position, string duration, string skills)
        {
            var role = string.IsNullOrWhiteSpace(position) ? "professional" : position;
            var employer = string.IsNullOrWhiteSpace(company) ? "the organization" : company;
            var period = string.IsNullOrWhiteSpace(duration) ? "multiple years" : duration;
            var capability = string.IsNullOrWhiteSpace(skills) ? "cross-functional execution and process improvement" : skills;

            return $"- Worked as {role} at {employer} for {period}.\n- Delivered measurable outcomes using {capability}.\n- Collaborated with teams, improved workflows and supported business goals.";
        }

        public async Task<string> GenerateExperienceBulletsFromPrompt(string prompt, string company, string position, string duration, string skills)
        {
            var employer = string.IsNullOrWhiteSpace(company) ? "the company" : company;
            var role = string.IsNullOrWhiteSpace(position) ? "the role" : position;
            var period = string.IsNullOrWhiteSpace(duration) ? "the employment period" : duration;
            var capability = string.IsNullOrWhiteSpace(skills) ? "team delivery, planning and problem solving" : skills;
            var focus = NormalizePrompt(prompt, "delivering reliable impact");

            var fallback = $"- Served as {role} at {employer} during {period}.\n- Applied {capability} to improve outcomes, teamwork and execution quality.\n- Stayed focused on {focus} while supporting business goals and day-to-day delivery.";

            var userPrompt = $"""
Write 3 strong resume responsibility bullets.
User instruction: {focus}
Company: {employer}
Role: {role}
Duration: {period}
Skills and tools: {capability}

Keep each bullet concise, ATS-friendly, and action-oriented. Return only the bullet list.
""";

            return await GenerateTextWithFallbackAsync(
                "You write ATS-friendly experience bullets for resumes. Return exactly a plain bullet list and do not include introductions or markdown code fences.",
                userPrompt,
                fallback);
        }

        public async Task<string> GenerateAchievementHighlights(string position, string skills)
        {
            var role = string.IsNullOrWhiteSpace(position) ? "the role" : position;
            var capability = string.IsNullOrWhiteSpace(skills) ? "operations, communication and execution" : skills;

            return $"- Increased efficiency and ownership in {role} responsibilities.\n- Supported better results through {capability}.\n- Contributed to team delivery with consistent performance and accountability.";
        }

        public async Task<string> GenerateAchievementHighlightsFromPrompt(string prompt, string position, string skills)
        {
            var role = string.IsNullOrWhiteSpace(position) ? "the role" : position;
            var capability = string.IsNullOrWhiteSpace(skills) ? "execution, communication and process improvement" : skills;
            var focus = NormalizePrompt(prompt, "improved efficiency and stronger delivery");

            var fallback = $"- Drove stronger results in {role} through {capability}.\n- Helped the team achieve {focus}.\n- Built trust through ownership, consistency and practical problem solving.";

            var userPrompt = $"""
Write 3 short resume achievement bullets.
User instruction: {focus}
Role: {role}
Skills and strengths: {capability}

The bullets should sound measurable and credible, even when exact numbers are not available. Return only the bullet list.
""";

            return await GenerateTextWithFallbackAsync(
                "You write concise achievement bullets for resumes. Focus on impact, ownership, and delivery. Return only bullet points.",
                userPrompt,
                fallback);
        }

        public async Task<string> GenerateEducationDetails(string degree, string university, string year, string gpa)
        {
            var degreeName = string.IsNullOrWhiteSpace(degree) ? "a professional degree" : degree;
            var institute = string.IsNullOrWhiteSpace(university) ? "a recognized institution" : university;
            var passingYear = string.IsNullOrWhiteSpace(year) ? "recent years" : year;
            var grade = string.IsNullOrWhiteSpace(gpa) ? "strong academic standing" : $"GPA/CGPA {gpa}";

            return $"{degreeName} completed from {institute} in {passingYear} with {grade}. Built academic foundation through projects, coursework and continuous learning.";
        }

        public async Task<string> GenerateEducationDetailsFromPrompt(string prompt, string degree, string university, string year, string gpa)
        {
            var degreeName = string.IsNullOrWhiteSpace(degree) ? "a professional degree" : degree;
            var institute = string.IsNullOrWhiteSpace(university) ? "a well-regarded institution" : university;
            var passingYear = string.IsNullOrWhiteSpace(year) ? "recent years" : year;
            var grade = string.IsNullOrWhiteSpace(gpa) ? "solid academic performance" : $"a GPA/CGPA of {gpa}";
            var focus = NormalizePrompt(prompt, "practical learning, discipline and continuous development");

            var fallback = $"{degreeName} from {institute}, completed in {passingYear}, with {grade}. Built strong foundations in {focus} through coursework, academic projects and focused study.";

            var userPrompt = $"""
Write a polished resume education description in 2 to 3 sentences.
User instruction: {focus}
Degree: {degreeName}
Institution: {institute}
Passing year: {passingYear}
Result: {grade}

Keep it ATS-friendly and suitable for a professional resume. Return only the final text.
""";

            return await GenerateTextWithFallbackAsync(
                "You write concise education descriptions for resumes. Keep the response professional, clear, and ready to paste.",
                userPrompt,
                fallback);
        }

        public async Task<int> CalculateATSScore(string skills, string experience, string education)
        {
            await Task.Delay(300);

            int score = 50;

            if (!string.IsNullOrWhiteSpace(skills)) score += 20;
            if (!string.IsNullOrWhiteSpace(experience)) score += 20;
            if (!string.IsNullOrWhiteSpace(education)) score += 10;

            if (score > 100) score = 100;

            return score;
        }

        public async Task<string> GenerateCoverLetter(string name, string jobTitle)
        {
            var fallback = $"Dear Hiring Manager,\n\nI am excited to apply for the {jobTitle} position. My skills and experience make me a strong candidate for this role.\n\nSincerely,\n{name}";

            var userPrompt = $"""
Write a concise professional cover letter.
Candidate name: {name}
Job title: {jobTitle}

Keep it practical, polished, and ready to send.
""";

            return await GenerateTextWithFallbackAsync(
                "You write clear professional cover letters. Return only the cover letter body with greeting and sign-off.",
                userPrompt,
                fallback);
        }

        public async Task<string> GenerateCoverLetterFromResume(string candidateName, string targetRole, string companyName, string resumeText)
        {
            var person = string.IsNullOrWhiteSpace(candidateName) ? "Candidate" : candidateName;
            var role = string.IsNullOrWhiteSpace(targetRole) ? "the role" : targetRole;
            var company = string.IsNullOrWhiteSpace(companyName) ? "your company" : companyName;
            var strengths = ExtractKeywords(resumeText, 5);
            var summary = strengths.Any() ? string.Join(", ", strengths) : "problem solving, ownership and collaboration";

            var fallback = $"Dear Hiring Manager,\n\nI am excited to apply for the {role} position at {company}. My background highlights strengths in {summary}, and I am confident those skills would help me contribute meaningful value to your team.\n\nAcross my experience, I have focused on reliable execution, continuous learning and supporting strong outcomes. I would welcome the opportunity to bring that same energy and discipline to {company}.\n\nSincerely,\n{person}";

            var userPrompt = $"""
Write a personalized cover letter using this resume context.
Candidate: {person}
Target role: {role}
Company: {company}
Resume text:
{resumeText}

Keep it concise, professional, and tailored to the role.
""";

            return await GenerateTextWithFallbackAsync(
                "You write tailored, professional cover letters from resume content. Return only the final cover letter.",
                userPrompt,
                fallback);
        }

        public async Task<(int Score, string Feedback)> AnalyzeAtsScore(string resumeText, string jobDescription)
        {
            await Task.Delay(250);

            var resumeWords = ExtractKeywords(resumeText, 20);
            var jobWords = ExtractKeywords(jobDescription, 20);
            var overlap = jobWords.Intersect(resumeWords).ToList();

            var score = 40;
            if (!string.IsNullOrWhiteSpace(resumeText)) score += 20;
            if (!string.IsNullOrWhiteSpace(jobDescription)) score += 10;
            score += Math.Min(30, overlap.Count * 5);
            score = Math.Min(score, 100);

            var missing = jobWords.Except(overlap).Take(6).ToList();
            var feedback = $"Matched keywords: {(overlap.Any() ? string.Join(", ", overlap) : "none yet")}.\n" +
                           $"Missing signals: {(missing.Any() ? string.Join(", ", missing) : "very few major gaps")}.\n" +
                           "Improve the score by aligning your summary, skills and experience bullets with the job description.";

            return (score, feedback);
        }

        public async Task<string> GenerateInterviewQuestions(string candidateName, string targetRole, string resumeText)
        {
            var person = string.IsNullOrWhiteSpace(candidateName) ? "the candidate" : candidateName;
            var role = string.IsNullOrWhiteSpace(targetRole) ? "this role" : targetRole;
            var keywords = ExtractKeywords(resumeText, 4);
            var focus = keywords.Any() ? string.Join(", ", keywords) : "your experience, strengths and problem solving";

            var fallback = $"1. Walk me through your background and why you are a fit for {role}.\n" +
                           $"2. Tell me about a project or responsibility where you used {focus}.\n" +
                           $"3. What measurable result did you create in your last role?\n" +
                           $"4. What challenge did you face recently and how did you solve it?\n" +
                           $"5. Why should we choose {person} for {role}?";

            var userPrompt = $"""
Generate 5 interview questions for a candidate.
Candidate: {person}
Target role: {role}
Resume focus areas: {focus}
Resume text:
{resumeText}

Questions should be practical and role-relevant. Return only a numbered list.
""";

            return await GenerateTextWithFallbackAsync(
                "You are an interview coach. Write sharp, relevant interview questions. Return only the numbered questions.",
                userPrompt,
                fallback);
        }

        public async Task<(int Score, string Feedback, string Mistakes)> EvaluateInterviewAnswers(string resumeText, string answers)
        {
            await Task.Delay(300);

            var score = 10;
            if (!string.IsNullOrWhiteSpace(answers)) score += 12;
            if (answers.Contains("result", StringComparison.OrdinalIgnoreCase) || answers.Contains("improve", StringComparison.OrdinalIgnoreCase)) score += 8;
            if (answers.Contains("team", StringComparison.OrdinalIgnoreCase) || answers.Contains("project", StringComparison.OrdinalIgnoreCase)) score += 8;
            if (answers.Length > 300) score += 6;
            if (Regex.IsMatch(answers, @"\d")) score += 6;
            score = Math.Min(score, 50);

            var mistakes = new List<string>();
            if (!answers.Contains("I ", StringComparison.OrdinalIgnoreCase)) mistakes.Add("Use stronger first-person ownership instead of generic statements.");
            if (!Regex.IsMatch(answers, @"\d")) mistakes.Add("Add measurable outcomes or numbers where possible.");
            if (!answers.Contains("challenge", StringComparison.OrdinalIgnoreCase) && !answers.Contains("problem", StringComparison.OrdinalIgnoreCase))
                mistakes.Add("Mention at least one challenge and how you solved it.");
            if (!answers.Contains("team", StringComparison.OrdinalIgnoreCase))
                mistakes.Add("Show collaboration and communication, not only individual work.");

            var feedback = "Your answers are reviewed for clarity, ownership, evidence and role alignment. Strong answers usually mention context, action and measurable result.";
            var mistakeText = mistakes.Any() ? string.Join("\n", mistakes.Select(x => $"- {x}")) : "- Good balance of ownership, detail and structure.";

            return (score, feedback, mistakeText);
        }

        public async Task<(int Score, string Feedback)> EvaluateWritingExam(string prompt, string answer)
        {
            await Task.Delay(250);

            var score = 40;
            if (!string.IsNullOrWhiteSpace(answer)) score += 20;
            if (answer.Length > 250) score += 15;
            if (answer.Contains("because", StringComparison.OrdinalIgnoreCase) || answer.Contains("therefore", StringComparison.OrdinalIgnoreCase)) score += 10;
            if (answer.Contains("for example", StringComparison.OrdinalIgnoreCase) || Regex.IsMatch(answer, @"\d")) score += 10;
            score = Math.Min(score, 100);

            var feedback = $"Prompt focus: {NormalizePrompt(prompt, "writing quality and relevance")}.\n" +
                           "Higher scores come from a clear opening, logical structure, specific examples and polished grammar.";

            return (score, feedback);
        }

        private async Task<string> GenerateTextWithFallbackAsync(string developerInstruction, string userPrompt, string fallback)
        {
            var apiKey = _configuration["OpenAI:ApiKey"];

            if (string.IsNullOrWhiteSpace(apiKey))
                apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                _logger.LogInformation("OpenAI API key was not configured. Using local fallback generation.");
                return fallback;
            }

            var model = _configuration["OpenAI:Model"];
            if (string.IsNullOrWhiteSpace(model))
                model = "gpt-5-mini";

            var baseUrl = _configuration["OpenAI:BaseUrl"];
            if (string.IsNullOrWhiteSpace(baseUrl))
                baseUrl = "https://api.openai.com/v1/";

            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(baseUrl), "responses"));
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
                request.Content = new StringContent(JsonSerializer.Serialize(new
                {
                    model,
                    max_output_tokens = 500,
                    input = new object[]
                    {
                        new
                        {
                            role = "developer",
                            content = new object[]
                            {
                                new
                                {
                                    type = "input_text",
                                    text = developerInstruction
                                }
                            }
                        },
                        new
                        {
                            role = "user",
                            content = new object[]
                            {
                                new
                                {
                                    type = "input_text",
                                    text = userPrompt
                                }
                            }
                        }
                    }
                }), Encoding.UTF8, "application/json");

                using var response = await _http.SendAsync(request);
                var json = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("OpenAI request failed with status {StatusCode}: {ResponseBody}", response.StatusCode, json);
                    return fallback;
                }

                var generatedText = CleanGeneratedText(ExtractResponseText(json));
                return string.IsNullOrWhiteSpace(generatedText) ? fallback : generatedText;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "OpenAI request failed. Falling back to local resume text generation.");
                return fallback;
            }
        }

        private static string ExtractResponseText(string json)
        {
            using var document = JsonDocument.Parse(json);

            if (document.RootElement.TryGetProperty("output_text", out var outputTextProperty)
                && outputTextProperty.ValueKind == JsonValueKind.String)
            {
                return outputTextProperty.GetString() ?? string.Empty;
            }

            if (!document.RootElement.TryGetProperty("output", out var output) || output.ValueKind != JsonValueKind.Array)
                return string.Empty;

            var textParts = new List<string>();

            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var contentItem in content.EnumerateArray())
                {
                    if (!contentItem.TryGetProperty("type", out var typeProperty))
                        continue;

                    if (!string.Equals(typeProperty.GetString(), "output_text", StringComparison.OrdinalIgnoreCase))
                        continue;

                    if (!contentItem.TryGetProperty("text", out var textProperty))
                        continue;

                    var value = textProperty.GetString();
                    if (!string.IsNullOrWhiteSpace(value))
                        textParts.Add(value.Trim());
                }
            }

            return string.Join("\n", textParts);
        }

        private static string CleanGeneratedText(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var cleaned = text.Trim();

            if (cleaned.StartsWith("```", StringComparison.Ordinal))
            {
                cleaned = Regex.Replace(cleaned, @"^```[a-zA-Z0-9_-]*\s*", string.Empty);
                cleaned = Regex.Replace(cleaned, @"\s*```$", string.Empty);
            }

            return cleaned.Trim();
        }

        private static string NormalizePrompt(string prompt, string fallback)
        {
            return string.IsNullOrWhiteSpace(prompt) ? fallback : prompt.Trim().TrimEnd('.', '!', '?');
        }

        private static List<string> ExtractKeywords(string text, int take)
        {
            if (string.IsNullOrWhiteSpace(text))
                return new List<string>();

            var stopWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "the","and","for","with","that","have","from","your","this","will","into","are","was","were","about","role","team","work","using","used","over","through","our","their","them","than","then","when","where","while","also","able","built","strong","skills","experience"
            };

            return Regex.Matches(text.ToLowerInvariant(), @"[a-z][a-z\+\#\-]{2,}")
                .Select(m => m.Value)
                .Where(x => !stopWords.Contains(x))
                .GroupBy(x => x)
                .OrderByDescending(g => g.Count())
                .ThenBy(g => g.Key)
                .Take(take)
                .Select(g => g.Key)
                .ToList();
        }
    }
}
