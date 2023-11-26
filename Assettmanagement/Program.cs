/*
 * We are using EF so to createt eh DB from scratch do this...
 * dotnet ef migrations add InitialCreate
 * dotnet ef database update
 * dotnet ef migrations add SeedSystemUser
 * dotnet ef database update
 * 
*/
using Assettmanagement.Data;
using Assettmanagement.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// in the Json file ...builder.WebHost.UseUrls("http://*:5000", "https://*:5001");
// Add services to the container.

//builder.Services.AddTransient<AccessDatabase>();
//builder.Services.AddTransient<IDataAccess>();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.SingleLine = true;
    options.TimestampFormat = "hh:mm:ss ";
});

builder.Services.AddRazorPages(options =>
{
    options.Conventions.AuthorizeFolder("/");
});

builder.Services.AddAuthorization(options =>
{
  //options.AddPolicy("SpecificUserOnly", policy => policy.RequireClaim(ClaimTypes.Name, "System"));
    options.AddPolicy("AdministratorOnly", policy => policy.RequireClaim("IsAdministrator", "true"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
// setup the link for Entity framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SqliteDatabase")));
builder.Services.AddTransient<IDataAccess>();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
string SipAddress = SecurityHelper.GetLocalIPAddress();
logger.LogInformation($"Local IP Address is {SipAddress}.");
// Create the database if it doesn't exist
//var accessDatabase = app.Services.GetService<AccessDatabase>();
//accessDatabase.CreateDatabaseIfNotExists();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

// Set the default route to the login page
app.MapGet("/", async context =>
{
    if (!context.User.Identity.IsAuthenticated)
    {
        context.Response.Redirect("/Login");
    }
    else
    {
        // If the user is authenticated, you can let them access the root URL
        // or redirect them to another page, e.g., a dashboard.
         context.Response.Redirect("Index");

        //return RedirectToPage("./Index");
    }
    await Task.CompletedTask;
});


app.Run();
