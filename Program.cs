using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResuniqAI.Data;
using ResuniqAI.Services;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// IMPORTANT: Disable reload file watcher issue
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddHttpClient();

QuestPDF.Settings.License = LicenseType.Community;

var app = builder.Build();


// ================= SAFE SEED (NO MIGRATION ON RENDER)
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    if (!await roleManager.RoleExistsAsync("Admin"))
        await roleManager.CreateAsync(new IdentityRole("Admin"));

    if (!await roleManager.RoleExistsAsync("Pro"))
        await roleManager.CreateAsync(new IdentityRole("Pro"));

    var adminEmail = "admin@resuniq.ai";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        var user = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(user, "Admin@123");
        await userManager.AddToRoleAsync(user, "Admin");
    }
}

// ================= ENV SAFE PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    // ONLY DEV: migration allowed
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
