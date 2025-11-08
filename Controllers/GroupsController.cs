using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebApp.Controllers
{
    [Authorize]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ILanguageService _languageService;
        private readonly ITeacherService _teacherService;
        public GroupsController(IGroupService groupService, ILanguageService languageService, ITeacherService teacherService)
        {
            _groupService = groupService;
            _languageService = languageService;
            _teacherService = teacherService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SearchGroups(int? languageId, int? teacherId)
        {
            var groups = await _groupService.SearchGroupsAsync(languageId, teacherId);
            return PartialView("_GroupsPartial", groups);
        }
    }
}