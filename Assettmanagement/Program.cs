using Assettmanagement.Data;
using Assettmanagement.Database;
using Assettmanagement.Security;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
// in the Json file ...builder.WebHost.UseUrls("http://*:5000", "https://*:5001");
// Add services to the container.
builder.Services.AddTransient<AccessDatabase>();
builder.Services.AddTransient<IDataAccess>();
//builder.Services.AddRazorPages();
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

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
string SipAddress = SecurityHelper.GetLocalIPAddress();
logger.LogInformation($"Local IP Address is {SipAddress}.");
// Create the database if it doesn't exist
var accessDatabase = app.Services.GetService<AccessDatabase>();
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
