namespace ResuniqAI.ViewModels
{
    public class InterviewCoachViewModel
    {
        public string CandidateName { get; set; } = "";
        public string ResumeText { get; set; } = "";
        public string TargetRole { get; set; } = "";
        public string InterviewQuestions { get; set; } = "";
        public string InterviewAnswers { get; set; } = "";
        public int InterviewScore { get; set; }
        public int InterviewScoreMax { get; set; } = 50;
        public string InterviewFeedback { get; set; } = "";
        public string MistakesFound { get; set; } = "";
        public string WritingPrompt { get; set; } = "";
        public string WritingAnswer { get; set; } = "";
        public int WritingScore { get; set; }
        public string WritingFeedback { get; set; } = "";
    }
}
