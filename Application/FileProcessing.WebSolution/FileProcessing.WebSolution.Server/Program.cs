//// SpaProxy loading ko programmatically stop karne ke liye top par add karein
//Environment.SetEnvironmentVariable("ASPNETCORE_HOSTINGSTARTUPASSEMBLIES", string.Empty);

//regular Program.cs code
using FileProcessing.Infrastructure.Persistence.Composition;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDbContextDependencyInjection((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
});
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.MapFallbackToFile("/index.html");
Console.WriteLine("Server started");
app.Run();



static string GetConnectionString(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!connectionString.IsNullOrEmpty())
    {
        return connectionString!;
    }
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}