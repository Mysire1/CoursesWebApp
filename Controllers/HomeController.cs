using CoursesWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoursesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentService? _studentService;
        private readonly ILanguageService? _languageService;
        private readonly ITeacherService? _teacherService;
        private readonly IGroupService? _groupService;

        public HomeController(ILogger<HomeController> logger,
                            IStudentService? studentService = null,
                            ILanguageService? languageService = null,
                            ITeacherService? teacherService = null,
                            IGroupService? groupService = null)
        {
            _logger = logger;
            _studentService = studentService;
            _languageService = languageService;
            _teacherService = teacherService;
            _groupService = groupService;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                _logger.LogInformation("Attempting to load home page data...");
                
                int studentsCount = 0;
                int languagesCount = 0;
                int teachersCount = 0;
                int groupsCount = 0;

                if (_studentService != null)
                {
                    var students = await _studentService.GetAllStudentsAsync();
                    studentsCount = students.Count();
                    _logger.LogInformation($"Loaded {studentsCount} students");
                }
                
                if (_languageService != null)
                {
                    var languages = await _languageService.GetAllLanguagesAsync();
                    languagesCount = languages.Count();
                    _logger.LogInformation($"Loaded {languagesCount} languages");
                }
                
                if (_teacherService != null)
                {
                    var teachers = await _teacherService.GetAllTeachersAsync();
                    teachersCount = teachers.Count();
                    _logger.LogInformation($"Loaded {teachersCount} teachers");
                }
                
                if (_groupService != null)
                {
                    var groups = await _groupService.GetAllGroupsAsync();
                    groupsCount = groups.Count();
                    _logger.LogInformation($"Loaded {groupsCount} groups");
                }

                ViewBag.StudentsCount = studentsCount;
                ViewBag.LanguagesCount = languagesCount;
                ViewBag.TeachersCount = teachersCount;
                ViewBag.GroupsCount = groupsCount;
                ViewBag.DatabaseConnected = true;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading home page data: {Message}", ex.Message);
                
                ViewBag.StudentsCount = 0;
                ViewBag.LanguagesCount = 0;
                ViewBag.TeachersCount = 0;
                ViewBag.GroupsCount = 0;
                ViewBag.DatabaseConnected = false;
                ViewBag.ErrorMessage = $"Database connection error: {ex.Message}";
                
                return View();
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}