using ResuniqAI.Models;

namespace ResuniqAI.Helpers
{
    public static class ResumeTemplateCatalog
    {
        private static readonly IReadOnlyList<ResumeTemplateDefinition> Templates = new List<ResumeTemplateDefinition>
        {
            new() { Key = "ats-clean", Name = "ATS Clean", Category = "ATS Friendly", Accent = "#0f766e", Description = "Single-column structure with clean hierarchy for recruiter systems.", BestFor = "General applications and freshers", Tone = "Clear and direct", Variant = "minimal", AtsScore = 98, Highlights = new[] { "Single column", "Keyword ready", "Fast scanning" }, SampleName = "Nadia Rahman", SampleRole = "Junior Software Engineer", SampleSummary = "Clean developer profile with practical projects, clear skills and strong ATS readability." },
            new() { Key = "ats-focus", Name = "ATS Focus", Category = "ATS Friendly", Accent = "#0b6e4f", Description = "Compact layout that prioritizes impact, skills and measurable outcomes.", BestFor = "Operations and support roles", Tone = "Results oriented", Variant = "minimal", AtsScore = 97, Highlights = new[] { "Impact bullets", "Skill grouping", "Lightweight styling" }, SampleName = "Arif Hasan", SampleRole = "Operations Executive", SampleSummary = "Outcome-driven resume focused on delivery speed, accuracy and process support." },
            new() { Key = "ats-precision", Name = "ATS Precision", Category = "ATS Friendly", Accent = "#2563eb", Description = "Balanced spacing and classic headings for modern job portals.", BestFor = "Corporate applications", Tone = "Professional", Variant = "executive", AtsScore = 96, Highlights = new[] { "Corporate friendly", "Structured summary", "Strong readability" }, SampleName = "Sarah Khan", SampleRole = "Business Analyst", SampleSummary = "Structured analyst resume with polished hierarchy and recruiter-friendly clarity." },
            new() { Key = "ats-classic", Name = "ATS Classic", Category = "ATS Friendly", Accent = "#1d4ed8", Description = "Traditional resume design with emphasis on chronology and fit.", BestFor = "Finance, admin and banking", Tone = "Formal", Variant = "executive", AtsScore = 95, Highlights = new[] { "Chronological flow", "Conservative styling", "Trusted format" }, SampleName = "Imran Chowdhury", SampleRole = "Accounts Officer", SampleSummary = "Formal layout with trusted structure for conservative hiring teams." },
            new() { Key = "ats-tech-grid", Name = "ATS Tech Grid", Category = "ATS Friendly", Accent = "#0369a1", Description = "Built for technical applicants with room for tools, systems and projects.", BestFor = "Software and IT roles", Tone = "Technical", Variant = "split", AtsScore = 96, Highlights = new[] { "Tech stack blocks", "Project callouts", "Structured keywords" }, SampleName = "Mira Dutta", SampleRole = "Full Stack Developer", SampleSummary = "Technical profile that showcases stack depth, projects and execution detail." },
            new() { Key = "ats-rapid", Name = "ATS Rapid", Category = "ATS Friendly", Accent = "#4f46e5", Description = "Lean format designed for high-volume job applications.", BestFor = "Mass outreach and quick apply", Tone = "Concise", Variant = "minimal", AtsScore = 94, Highlights = new[] { "Quick edit", "Minimal clutter", "High compatibility" }, SampleName = "Fahim Noor", SampleRole = "Sales Associate", SampleSummary = "Fast-moving resume layout with concise sections and quick-read impact." },

            new() { Key = "executive-edge", Name = "Executive Edge", Category = "Premium Formats", Accent = "#7c3aed", Description = "Boardroom-inspired format with strong leadership presence.", BestFor = "Senior managers and directors", Tone = "Executive", Variant = "executive", AtsScore = 90, Highlights = new[] { "Leadership framing", "Revenue-first sections", "Refined spacing" }, SampleName = "Reza Karim", SampleRole = "Director of Operations", SampleSummary = "Leadership-focused design with strong authority, strategy and performance framing." },
            new() { Key = "vision-pro", Name = "Vision Pro", Category = "Premium Formats", Accent = "#db2777", Description = "Modern visual layout for product and strategy professionals.", BestFor = "Product, growth and strategy", Tone = "Modern", Variant = "split", AtsScore = 88, Highlights = new[] { "Modern branding", "Role snapshots", "Balanced layout" }, SampleName = "Tania Sultana", SampleRole = "Product Manager", SampleSummary = "Contemporary sample resume with clear strategy, roadmap and cross-team leadership cues." },
            new() { Key = "graphite", Name = "Graphite", Category = "Premium Formats", Accent = "#334155", Description = "Elegant monochrome design for consulting and business teams.", BestFor = "Consulting and business ops", Tone = "Elegant", Variant = "executive", AtsScore = 89, Highlights = new[] { "Low-noise palette", "Premium spacing", "Confident headers" }, SampleName = "Nabil Ahmed", SampleRole = "Consultant", SampleSummary = "Elegant sample layout with premium spacing and clean consulting tone." },
            new() { Key = "neo-creative", Name = "Neo Creative", Category = "Premium Formats", Accent = "#ea580c", Description = "Bold format with story-driven sections for creative applicants.", BestFor = "Marketing, design and content", Tone = "Bold", Variant = "split", AtsScore = 84, Highlights = new[] { "Creative energy", "Narrative summary", "Portfolio emphasis" }, SampleName = "Ava Sen", SampleRole = "Brand Designer", SampleSummary = "Visual-forward sample with portfolio energy and narrative-led profile sections." },
            new() { Key = "product-flow", Name = "Product Flow", Category = "Premium Formats", Accent = "#0891b2", Description = "Smooth hierarchy built for PMs, analysts and startup teams.", BestFor = "Product and startup roles", Tone = "Strategic", Variant = "timeline", AtsScore = 87, Highlights = new[] { "Milestone flow", "Outcome centric", "Clean metrics" }, SampleName = "Rafi Islam", SampleRole = "Product Analyst", SampleSummary = "Timeline-style sample format built around milestones, outcomes and product thinking." },
            new() { Key = "studio-line", Name = "Studio Line", Category = "Premium Formats", Accent = "#be185d", Description = "Portfolio-led format for designers and creative technologists.", BestFor = "Creative studio roles", Tone = "Expressive", Variant = "split", AtsScore = 82, Highlights = new[] { "Portfolio CTA", "Visual balance", "Brand-forward layout" }, SampleName = "Lina Roy", SampleRole = "UI Designer", SampleSummary = "Portfolio-friendly sample design with expressive headers and soft visual rhythm." },
            new() { Key = "urban-slate", Name = "Urban Slate", Category = "Premium Formats", Accent = "#0f172a", Description = "Neutral, stylish template for high-end modern resumes.", BestFor = "Cross-functional professionals", Tone = "Refined", Variant = "minimal", AtsScore = 88, Highlights = new[] { "Versatile look", "Subtle branding", "Modern readability" }, SampleName = "Tanvir Alam", SampleRole = "Project Coordinator", SampleSummary = "Refined sample layout with modern contrast and wide readability." },
            new() { Key = "pulse", Name = "Pulse", Category = "Premium Formats", Accent = "#dc2626", Description = "Action-heavy format that gives energy to achievement bullets.", BestFor = "Sales and growth roles", Tone = "Energetic", Variant = "timeline", AtsScore = 85, Highlights = new[] { "Achievement led", "Momentum sections", "Easy scanning" }, SampleName = "Samiha Akter", SampleRole = "Growth Executive", SampleSummary = "High-energy sample focused on wins, momentum and persuasive achievement blocks." },
            new() { Key = "horizon", Name = "Horizon", Category = "Premium Formats", Accent = "#0284c7", Description = "Wide breathing room and clean bands for premium digital profiles.", BestFor = "Remote and global roles", Tone = "Contemporary", Variant = "minimal", AtsScore = 86, Highlights = new[] { "Open layout", "Remote-friendly", "Strong readability" }, SampleName = "Arian Mitra", SampleRole = "Remote Account Manager", SampleSummary = "Open, contemporary sample design with relaxed spacing and polished digital presence." },
            new() { Key = "craft-pro", Name = "Craft Pro", Category = "Premium Formats", Accent = "#15803d", Description = "Project-centered layout with clear proof of execution.", BestFor = "Engineers and builders", Tone = "Practical", Variant = "split", AtsScore = 89, Highlights = new[] { "Project sections", "Implementation focus", "Dense signal" }, SampleName = "Shihab Uddin", SampleRole = "Software Engineer", SampleSummary = "Build-focused sample format with dense project detail and practical execution proof." },
            new() { Key = "meridian", Name = "Meridian", Category = "Premium Formats", Accent = "#7c2d12", Description = "Warm premium template for customer-facing leadership roles.", BestFor = "Customer success and partnerships", Tone = "Warm", Variant = "executive", AtsScore = 87, Highlights = new[] { "People-first tone", "Leadership blocks", "Premium feel" }, SampleName = "Farzana Noor", SampleRole = "Customer Success Lead", SampleSummary = "Warm premium sample with people-first leadership framing and client-ready tone." },
            new() { Key = "apex", Name = "Apex", Category = "Premium Formats", Accent = "#4338ca", Description = "High-contrast corporate template for standout executive applications.", BestFor = "C-suite and VP roles", Tone = "Authoritative", Variant = "executive", AtsScore = 90, Highlights = new[] { "Authority-led", "Big metrics", "Professional polish" }, SampleName = "Mahin Siddique", SampleRole = "Vice President", SampleSummary = "High-contrast executive sample format built for authority, scale and business impact." },
            new() { Key = "mint-column", Name = "Mint Column", Category = "Premium Formats", Accent = "#0d9488", Description = "Fresh two-tone format with calm spacing for modern professionals.", BestFor = "Business, support and client-facing roles", Tone = "Clean", Variant = "split", AtsScore = 88, Highlights = new[] { "Balanced sidebar", "Soft contrast", "Clear section rhythm" }, SampleName = "Nusrat Jahan", SampleRole = "Customer Support Specialist", SampleSummary = "Clean premium sample with calm structure, modern contrast and easy readability." },
            new() { Key = "north-star", Name = "North Star", Category = "Premium Formats", Accent = "#1e3a8a", Description = "Confident leadership layout with strong top-header emphasis.", BestFor = "Team leads and senior specialists", Tone = "Confident", Variant = "executive", AtsScore = 89, Highlights = new[] { "Strong header", "Leadership framing", "Executive polish" }, SampleName = "Sakib Rahman", SampleRole = "Engineering Team Lead", SampleSummary = "Leadership-first sample design built for clarity, authority and modern team impact." },
            new() { Key = "signal-pro", Name = "Signal Pro", Category = "Premium Formats", Accent = "#c2410c", Description = "Sharp achievement-driven template for high-energy individual contributors.", BestFor = "Sales, marketing and growth roles", Tone = "Sharp", Variant = "timeline", AtsScore = 86, Highlights = new[] { "Fast scanning", "Bold sectioning", "Achievement emphasis" }, SampleName = "Maliha Ahmed", SampleRole = "Marketing Executive", SampleSummary = "Sharp premium sample that highlights achievements, momentum and campaign impact." }
        };

        public static IReadOnlyList<ResumeTemplateDefinition> GetAll() => Templates;
        public static IReadOnlyList<ResumeTemplateDefinition> GetAtsFriendly() => Templates.Where(x => x.Category == "ATS Friendly").ToList();
        public static IReadOnlyList<ResumeTemplateDefinition> GetPremium() => Templates.Where(x => x.Category == "Premium Formats").ToList();
        public static ResumeTemplateDefinition GetByKey(string? key) => Templates.FirstOrDefault(x => x.Key == key) ?? Templates.First(x => x.Key == "ats-clean");

        public static Resume CreateSampleResume(string? templateKey)
        {
            var template = GetByKey(templateKey);
            var techFocused = template.Key is "ats-tech-grid" or "craft-pro";
            var strategic = template.Key is "vision-pro" or "product-flow" or "executive-edge" or "apex";

            return new Resume
            {
                TemplateKey = template.Key,
                PageSize = "A4",
                FullName = template.SampleName,
                Email = "sample@resuniq.ai",
                Phone = "+880 1712-345678",
                Address = "Dhaka, Bangladesh",
                LinkedIn = "linkedin.com/in/sample-profile",
                Github = techFocused ? "github.com/sample-dev" : "",
                Portfolio = techFocused ? "sample.dev/portfolio" : "sampleportfolio.me",
                Summary = template.SampleSummary,
                Skills = techFocused
                    ? "C#, ASP.NET Core, Entity Framework, SQL Server, JavaScript, REST API, Git, Problem Solving"
                    : strategic
                        ? "Product Strategy, Stakeholder Management, Roadmapping, KPI Tracking, Communication, Leadership"
                        : "Communication, Teamwork, Microsoft Office, Reporting, Coordination, Time Management, Problem Solving",
                CompanyName = strategic ? "Orbit Labs" : "Resuniq Technologies",
                PositionTitle = template.SampleRole,
                EmploymentDuration = "2023 - Present",
                EmploymentLocation = "Dhaka, Bangladesh",
                EmploymentResponsibilities = techFocused
                    ? "Built internal web features using ASP.NET Core MVC.\nCollaborated with design and QA to ship reliable releases.\nMaintained clean code, reusable components and bug fixes."
                    : strategic
                        ? "Led cross-functional planning and execution for customer-facing initiatives.\nTracked outcomes, aligned stakeholders and improved delivery visibility.\nPrepared reports, presentations and action plans for leadership."
                        : "Supported daily operations and customer-facing activities.\nPrepared reports, maintained records and coordinated with internal teams.\nImproved process consistency through careful follow-up and documentation.",
                EmploymentAchievements = techFocused
                    ? "Reduced repetitive manual work by automating a reporting task.\nDelivered practical features used by internal teams.\nImproved issue resolution speed through better debugging and testing."
                    : strategic
                        ? "Improved project delivery rhythm with clearer planning.\nHelped teams focus on measurable outcomes.\nContributed to smoother stakeholder communication."
                        : "Maintained accurate documentation and timely follow-up.\nImproved task completion consistency across the team.\nSupported service quality with clear communication.",
                DegreeName = techFocused ? "BSc in Computer Science and Engineering" : "Bachelor of Business Administration",
                UniversityName = techFocused ? "Daffodil International University" : "University of Dhaka",
                PassingYear = "2023",
                Gpa = "3.78 / 4.00",
                EducationDetails = techFocused
                    ? "Focused on software engineering, databases and web application development. Completed multiple practical academic projects."
                    : "Focused on communication, analytics and practical teamwork through coursework and collaborative projects.",
                Projects = techFocused
                    ? "Project: Resume Builder Platform\nRole: Full Stack Developer\nTech Stack: ASP.NET Core MVC, Entity Framework Core, SQLite, Bootstrap\nDetails:\nBuilt a resume builder with template switching, AI-assisted content generation and PDF export."
                    : "Project: Customer Experience Dashboard\nRole: Project Coordinator\nDetails:\nCoordinated reporting inputs, streamlined updates and improved visibility for leadership.",
                LeadershipAndActivities = "Leadership Role: Campus Technology Club Coordinator\nDuration: 2022 - 2023\nDetails:\nOrganized workshops, coordinated volunteers and supported peer learning activities.",
                Certifications = techFocused
                    ? "Certification: ASP.NET Core Web Development\nIssuer: Online Learning Platform\nYear: 2024"
                    : "Certification: Business Communication Essentials\nIssuer: Professional Skills Academy\nYear: 2024",
                Achievements = techFocused
                    ? "Achievement: Built and shipped internal workflow tools that reduced repetitive manual tasks.\nYear: 2024"
                    : "Achievement: Recognized for reliable execution and strong cross-team coordination.\nYear: 2024",
                AdditionalInformation = techFocused
                    ? "Languages: English, Bangla\nAvailability: Immediate\nInterests: Open source, clean architecture, product building"
                    : "Languages: English, Bangla\nAvailability: Immediate\nInterests: Customer experience, operations, teamwork",
                Reference = "Available upon request"
            };
        }
    }
}
