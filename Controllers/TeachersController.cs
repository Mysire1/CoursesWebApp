using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeachersController : Controller
    {
        private readonly ITeacherService _teacherService;
        public TeachersController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            var teachers = await _teacherService.GetAllTeachersAsync();
            if (!string.IsNullOrEmpty(filter))
                teachers = teachers.Where(x => (x.FullName ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase) || (x.Email ?? "").Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList();
            ViewBag.Filter = filter;
            ViewBag.Teachers = teachers;
            return View();
        }
    }
}
