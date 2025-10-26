using CoursesWebApp.Data;
using CoursesWebApp.Services;
using CoursesWebApp.Services.Impl;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseNpgsql(connectionString);
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.LogTo(Console.WriteLine, LogLevel.Information);
    }
});

// Services
builder.Services.AddScoped<IStudentService, StudentServiceImpl>();
builder.Services.AddScoped<IGroupService, GroupServiceImpl>();
builder.Services.AddScoped<ILanguageService, LanguageServiceImpl>();
builder.Services.AddScoped<ITeacherService, TeacherServiceImpl>();
builder.Services.AddScoped<IQueryService, QueryServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Map routes FIRST
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Remove global fallback that was catching all requests and showing 404
// If you need a spa fallback later, add it after static files to a specific path

// Ensure database is created and reachable
try 
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    if (await context.Database.CanConnectAsync())
    {
        await context.Database.EnsureCreatedAsync();
        logger.LogInformation("Database connected and schema ensured");
    }
    else
    {
        logger.LogError("Cannot connect to database. Check appsettings.json");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error during DB initialization: {Message}", ex.Message);
}

app.Run();