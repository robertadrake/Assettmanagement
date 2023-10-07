using Assettmanagement.Data;
using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:5000", "https://*:5001");
// Add services to the container.
builder.Services.AddTransient<AccessDatabase>();
builder.Services.AddTransient<IDataAccess>();
//builder.Services.AddRazorPages();
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
