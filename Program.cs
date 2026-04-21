using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResuniqAI.Data;
using ResuniqAI.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// =====================
// CONNECTION STRING
// =====================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// =====================
// DB CONTEXT
// =====================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// =====================
// IDENTITY
// =====================
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// =====================
// MVC + RAZOR
// =====================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// =====================
// CUSTOM SERVICES
// =====================
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddHttpClient();

// =====================
// QUESTPDF LICENSE FIX (IMPORTANT)
// =====================
QuestPDF.Settings.License = LicenseType.Community;

// =====================
// BUILD APP
// =====================
var app = builder.Build();

// =====================
// PIPELINE
// =====================
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// IMPORTANT (CSS, JS, images)
app.UseStaticFiles();

app.UseRouting();

// AUTH (IMPORTANT ORDER)
app.UseAuthentication();
app.UseAuthorization();

// =====================
// ROUTES
// =====================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();