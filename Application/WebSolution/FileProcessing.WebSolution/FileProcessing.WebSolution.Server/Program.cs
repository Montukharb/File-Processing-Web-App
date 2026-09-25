using FileProcessing.Core.CommonLibrary.Exceptions;
using FileProcessing.Core.CommonLibrary.Extensions;
using FileProcessing.Infrastructure.Persistence;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement;
using FileProcessing.Infrastructure.Persistence.ApplicationUserManagement.Entitiy;
using FileProcessing.Infrastructure.Persistence.Composition;
using FileProcessing.WebSolution.ModuleComposition;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAppDbContextDependencyInjection((_, options) =>
{
    options.UseSqlServer(GetConnectionString(builder));
}).AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true; //by default true
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


//serilog configuration
builder.Host.ConfigureSerilog(builder.Configuration);
builder.Services.AddModuleComposition(builder.Configuration);
// Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy => policy.WithOrigins("http://localhost:4200") // Replace with your Angular app URL
                        .AllowAnyHeader()
                        .AllowAnyMethod());
});
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddResponseCompression();
var app = builder.Build();

//Register the global exception handler middleware
app.UseMiddleware<GlobalExceptionHandler>();
//app.UseAuthentication();
//app.UseAuthorization();
app.UseRateLimiter(); // Rate Limiter Middleware
app.MapControllers(); // Map the controllers to the request pipeline
app.UseDefaultFiles(); //Find and match the default file like index.html etc.
app.MapStaticAssets(); // Map the static assets folder to the request path "/assets"
app.UseStatusCodePages(); // Handle status code pages for 404, 500, etc.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "File Processing Api");
    });

}
// Configure the HTTP request pipeline.
app.UseHttpsRedirection(); //http request convert into https request
app.UseStaticFiles(); //https://localhost:5001/images/photo.jpg
//app.UseStaticFiles(new StaticFileOptions
//{
//    FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "PrivateAssets")),
//    RequestPath = "/assets" /* requestpath "/assets/image.png" client access */
//});

app.UseCors("AllowAngularApp");
app.UseResponseCompression(); //response json compress auto
//app.UseAuthentication();
//app.UseAuthorization();
app.MapGet("api/", () =>
{
    return TypedResults.Ok("working");
}).RequireRateLimiting("Fixed");
app.MapFallback(() => Results.NotFound("The requested resource was not found."));
Console.WriteLine("Server started");
app.Run();
static string GetConnectionString(WebApplicationBuilder builder)
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!string.IsNullOrEmpty(connectionString))
    {
        return connectionString!;
    }
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}