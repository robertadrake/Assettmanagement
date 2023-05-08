using Assettmanagement.Data;
using Assettmanagement.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<mySQLDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<AccessDatabase>();
builder.Services.AddScoped<DataAccess>(); // Keep either transient or scoped, but not both
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(); // Add this line to register the authorization services.
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<mySQLDbContext>();
builder.Services.AddRazorPages();
var app = builder.Build();

// Create the database if it doesn't exist
var accessDatabase = app.Services.GetService<AccessDatabase>();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();  // this is for login security
app.UseAuthorization();
app.MapRazorPages();
app.Run();
