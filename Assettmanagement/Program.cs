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
   options.AddPolicy("AdministratorOnly", policy => policy.RequireClaim("IsAdministrator", "true"));
});
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie();
// setup the link for Entity framework
builder.Services.AddDbContext<AppDbContext>(options =>options.UseSqlite(builder.Configuration.GetConnectionString("SqlDatabase")));
builder.Services.AddTransient<IDataAccess>();

var app = builder.Build();
// Initialize and seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    context.Database.Migrate(); // This will apply any pending migrations and create the database
}
var logger = app.Services.GetRequiredService<ILogger<Program>>();
string SipAddress = SecurityHelper.GetLocalIPAddress();
logger.LogInformation($"Local IP Address is {SipAddress}.");

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
