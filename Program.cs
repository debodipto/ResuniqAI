// Program startup configuration

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using QuestPDF.Infrastructure;
using ResuniqAI.Data;
using ResuniqAI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var connectionString =
builder.Configuration.GetConnectionString("DefaultConnection");

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


// Seed default roles and admin account
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var roleManager =
        scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    await dbContext.Database.MigrateAsync();
    await EnsureSqliteSchemaCompatibilityAsync(dbContext);

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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
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

static async Task EnsureSqliteSchemaCompatibilityAsync(ApplicationDbContext dbContext)
{
    var connection = dbContext.Database.GetDbConnection();
    await connection.OpenAsync();

    await EnsureTableExistsAsync(connection, "UserProfiles", """
CREATE TABLE IF NOT EXISTS "UserProfiles" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_UserProfiles" PRIMARY KEY AUTOINCREMENT,
    "UserId" TEXT NOT NULL,
    "FullName" TEXT NOT NULL,
    "Headline" TEXT NOT NULL,
    "Location" TEXT NOT NULL,
    "Phone" TEXT NOT NULL,
    "LinkedIn" TEXT NOT NULL,
    "Github" TEXT NOT NULL,
    "Portfolio" TEXT NOT NULL,
    "Bio" TEXT NOT NULL,
    "UpdatedAt" TEXT NOT NULL
);
""");

    await EnsureColumnExistsAsync(connection, "Resumes", "AchievementDetails", """ALTER TABLE "Resumes" ADD COLUMN "AchievementDetails" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "Achievements", """ALTER TABLE "Resumes" ADD COLUMN "Achievements" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "AdditionalInformation", """ALTER TABLE "Resumes" ADD COLUMN "AdditionalInformation" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "CertificationDetails", """ALTER TABLE "Resumes" ADD COLUMN "CertificationDetails" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "Certifications", """ALTER TABLE "Resumes" ADD COLUMN "Certifications" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "LeadershipActivityDetails", """ALTER TABLE "Resumes" ADD COLUMN "LeadershipActivityDetails" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "LeadershipAndActivities", """ALTER TABLE "Resumes" ADD COLUMN "LeadershipAndActivities" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "ProjectDetails", """ALTER TABLE "Resumes" ADD COLUMN "ProjectDetails" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "Projects", """ALTER TABLE "Resumes" ADD COLUMN "Projects" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "Reference", """ALTER TABLE "Resumes" ADD COLUMN "Reference" TEXT NOT NULL DEFAULT '';""");
    await EnsureColumnExistsAsync(connection, "Resumes", "ReferenceDetails", """ALTER TABLE "Resumes" ADD COLUMN "ReferenceDetails" TEXT NOT NULL DEFAULT '';""");
}

static async Task EnsureTableExistsAsync(System.Data.Common.DbConnection connection, string tableName, string createSql)
{
    await using var existsCommand = connection.CreateCommand();
    existsCommand.CommandText = """
SELECT COUNT(*)
FROM sqlite_master
WHERE type = 'table' AND name = $tableName;
""";

    var parameter = existsCommand.CreateParameter();
    parameter.ParameterName = "$tableName";
    parameter.Value = tableName;
    existsCommand.Parameters.Add(parameter);

    var exists = Convert.ToInt32(await existsCommand.ExecuteScalarAsync()) > 0;
    if (exists)
        return;

    await using var createCommand = connection.CreateCommand();
    createCommand.CommandText = createSql;
    await createCommand.ExecuteNonQueryAsync();
}

static async Task EnsureColumnExistsAsync(System.Data.Common.DbConnection connection, string tableName, string columnName, string alterSql)
{
    await using var command = connection.CreateCommand();
    command.CommandText = $"PRAGMA table_info(\"{tableName}\");";

    await using var reader = await command.ExecuteReaderAsync();
    while (await reader.ReadAsync())
    {
        if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
            return;
    }

    await reader.CloseAsync();

    await using var alterCommand = connection.CreateCommand();
    alterCommand.CommandText = alterSql;
    await alterCommand.ExecuteNonQueryAsync();
}
