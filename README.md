# ResuniqAI

ResuniqAI is an ASP.NET Core MVC web app for building resumes, generating AI-assisted content, exporting PDFs, managing user profiles, and handling premium template access through admin-approved payments.

## Tech Stack

- ASP.NET Core MVC on `.NET 10`
- ASP.NET Core Identity with roles (`Admin`, `Pro`)
- Entity Framework Core with SQLite
- QuestPDF for PDF generation
- OpenAI Responses API with local fallback generation when no API key is configured

## Solution Structure

- Root folder - active web application project
- `Controllers/` - MVC request flow
- `Services/` - AI and PDF services
- `Data/` - EF Core context and migrations
- `Models/` - database entities
- `ViewModels/` - page/editor view models
- `Helpers/` - template and page-size catalogs
- `Views/` - Razor views

## Process Flow

### 1. Application Startup Flow

```mermaid
flowchart TD
    A[App Start] --> B[Load appsettings.json]
    B --> C[Configure SQLite DbContext]
    C --> D[Configure Identity and Roles]
    D --> E[Register AIService and PdfService]
    E --> F[Build WebApplication]
    F --> G[Ensure database exists]
    G --> H[Seed roles: Admin, Pro]
    H --> I[Seed default admin account]
    I --> J[Configure middleware pipeline]
    J --> K[Map MVC routes and Razor Pages]
```

### 2. User Journey Flow

```mermaid
flowchart TD
    A[User Lands on Home Page] --> B{Signed in?}
    B -- No --> C[Register or Login via Identity]
    B -- Yes --> D[Open Resume Workspace]
    D --> E[Create or Edit Resume]
    E --> F[Choose ATS or Premium Template]
    F --> G{Need AI help?}
    G -- Yes --> H[Generate summary, experience, or education text]
    G -- No --> I[Save resume]
    H --> I
    I --> J[Preview or Download PDF]
    J --> K[Use Career Toolkit / Interview Coach / Job Tracker]
```

### 3. Resume Builder Process

```mermaid
flowchart TD
    A[Open Create or Edit Resume] --> B[Build ResumeEditorViewModel]
    B --> C[Load template catalog and page sizes]
    C --> D[Parse stored resume JSON into editor entries]
    D --> E[User edits resume sections]
    E --> F{AI command submitted?}
    F -- Yes --> G[AIService generates section text]
    G --> H[Update editor model]
    F -- No --> I[Normalize entries]
    H --> I
    I --> J[Serialize structured sections to JSON]
    J --> K[Build plain-text resume sections]
    K --> L[Save Resume entity to SQLite]
```

### 4. AI Feature Process

```mermaid
flowchart TD
    A[User submits AI action] --> B[Controller collects prompt and resume context]
    B --> C{OpenAI API key configured?}
    C -- Yes --> D[Call OpenAI Responses API]
    C -- No --> E[Use local fallback text generation]
    D --> F[Extract generated text]
    E --> F
    F --> G[Clean text and return result]
    G --> H[Controller updates view model or resume]
```

### 5. PDF Generation Process

```mermaid
flowchart TD
    A[User clicks Preview PDF or Download PDF] --> B[Load resume data]
    B --> C[Resolve template by TemplateKey]
    C --> D[Resolve page size]
    D --> E{Template variant}
    E -- minimal --> F[BuildMinimal layout]
    E -- executive --> G[BuildExecutive layout]
    E -- split --> H[BuildSplit layout]
    E -- timeline --> I[BuildTimeline layout]
    F --> J[QuestPDF generates byte array]
    G --> J
    H --> J
    I --> J
    J --> K[Return PDF file response]
```

### 6. Premium Upgrade Process

```mermaid
flowchart TD
    A[User opens Upgrade page] --> B[Submit transaction/reference ID]
    B --> C[Payment record created as pending]
    C --> D[Admin opens Payments panel]
    D --> E[Admin approves payment]
    E --> F[User is added to Pro role]
    F --> G[Premium templates become available]
```

### 7. Admin Process

```mermaid
flowchart TD
    A[Admin logs in] --> B[Open Admin Panel]
    B --> C[View totals: users, resumes, payments, jobs]
    C --> D[Manage payments]
    C --> E[Manage jobs]
    C --> F[Manage resumes]
    C --> G[View users]
    D --> H[Approve pending payment]
    E --> I[Create or toggle jobs]
    F --> J[Delete resume if needed]
```

## Main Functional Areas

### Resume Management

- Users can create, edit, list, and download resumes.
- Resume sections support both legacy text fields and structured JSON-backed editor entries.
- Template selection is built into the editor workflow.

Key files:

- `Controllers/ResumeController.cs`
- `ViewModels/ResumeEditorViewModel.cs`
- `Helpers/ResumeTemplateCatalog.cs`
- `Services/PdfService.cs`

### AI Features

- Resume summary generation
- Experience bullet generation
- Achievement highlight generation
- Education description generation
- Cover letter generation
- ATS scoring
- Interview question generation
- Interview answer scoring
- Writing exam scoring

Key files:

- `Services/AIService.cs`
- `Controllers/FeaturesController.cs`
- `Controllers/AIController.cs`

### User Profile

- Stores profile details like headline, phone, portfolio, GitHub, LinkedIn, and bio.
- Displays recent resumes and payment history in the profile dashboard.

Key file:

- `Controllers/ProfileController.cs`

### Admin and Payments

- Admin can approve payments.
- Approval upgrades the user to the `Pro` role.
- Admin can manage jobs, resumes, users, and basic revenue metrics.

Key files:

- `Controllers/AdminController.cs`
- `Controllers/PaymentController.cs`

## Database Entities

- `Resume`
- `Payment`
- `Subscription`
- `JobPosting`
- `UserProfile`
- ASP.NET Identity tables for users and roles

Defined in:

- `Data/ApplicationDbContext.cs`
- `Models/`

## Configuration

Application settings are stored in:

- `appsettings.json`
- `appsettings.Development.json`

Important configuration:

- SQLite connection string: `DefaultConnection`
- OpenAI settings:
  - `OpenAI:ApiKey`
  - `OpenAI:Model`
  - `OpenAI:BaseUrl`

## Run Locally

From the solution root:

```powershell
dotnet run --project .\ResuniqAI.csproj
```

Default local URLs from launch settings:

- `http://localhost:5160`
- `https://localhost:7073`

## Current Notes

- The active web project now runs directly from the repository root at `ResuniqAI.csproj`.
- The app seeds a default admin account during startup.
- Premium access is controlled by the `Pro` role.
- If no OpenAI API key is set, AI features still work with local fallback content generation.
