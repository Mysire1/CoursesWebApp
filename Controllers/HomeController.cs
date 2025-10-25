using CoursesWebApp.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CoursesWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IStudentService _studentService;
        private readonly ILanguageService _languageService;
        private readonly ITeacherService _teacherService;
        private readonly IGroupService _groupService;

        public HomeController(ILogger<HomeController> logger,
                            IStudentService studentService,
                            ILanguageService languageService,
                            ITeacherService teacherService,
                            IGroupService groupService)
        {
            _logger = logger;
            _studentService = studentService;
            _languageService = languageService;
            _teacherService = teacherService;
            _groupService = groupService;
        }

        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            var languages = await _languageService.GetAllLanguagesAsync();
            var teachers = await _teacherService.GetAllTeachersAsync();
            var groups = await _groupService.GetAllGroupsAsync();

            ViewBag.StudentsCount = students.Count();
            ViewBag.LanguagesCount = languages.Count();
            ViewBag.TeachersCount = teachers.Count();
            ViewBag.GroupsCount = groups.Count();

            return View();
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