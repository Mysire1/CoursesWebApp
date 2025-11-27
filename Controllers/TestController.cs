using CoursesWebApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TestController> _logger;

        public TestController(ApplicationDbContext context, ILogger<TestController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var result = new List<string>();
            
            try
            {
                result.Add("=== DIAGNOSTIC RESULTS ===");
                result.Add($"Current Time: {DateTime.Now}");
                result.Add($"Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
                
                result.Add("\nTesting database connection...");
                
                var canConnect = await _context.Database.CanConnectAsync();
                result.Add($"Can connect to database: {canConnect}");
                
                if (canConnect)
                {
                    result.Add("\nQuerying tables...");
                    
                    try
                    {
                        var languagesCount = await _context.Languages.CountAsync();
                        result.Add($"Languages count: {languagesCount}");
                    }
                    catch (Exception ex)
                    {
                        result.Add($"Languages query failed: {ex.Message}");
                    }
                    
                    try
                    {
                        var studentsCount = await _context.Students.CountAsync();
                        result.Add($"Students count: {studentsCount}");
                    }
                    catch (Exception ex)
                    {
                        result.Add($"Students query failed: {ex.Message}");
                    }
                    
                    try
                    {
                        var teachersCount = await _context.Teachers.CountAsync();
                        result.Add($"Teachers count: {teachersCount}");
                    }
                    catch (Exception ex)
                    {
                        result.Add($"Teachers query failed: {ex.Message}");
                    }
                }
                
            }
            catch (Exception ex)
            {
                result.Add($"\nERROR: {ex.Message}");
                result.Add($"Stack Trace: {ex.StackTrace}");
            }
            
            ViewBag.Results = result;
            return View();
        }
        
        public IActionResult Simple()
        {
            ViewBag.Message = "Test controller is working!";
            ViewBag.Time = DateTime.Now.ToString();
            return View();
        }
    }
}