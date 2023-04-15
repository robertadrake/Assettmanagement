using Assettmanagement.Data;
using Assettmanagement.Database;
using Assettmanagement.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddTransient<AccessDatabase>();
builder.Services.AddTransient<DataAccess>();
builder.Services.AddRazorPages();
builder.Services.AddAuthorization(); // Add this line to register the authorization services.

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
app.UseAuthorization();

app.MapRazorPages();

app.Run();
