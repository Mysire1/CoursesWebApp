using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(GroupCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
                ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
                return View(vm);
            }
            var group = new Group {
                GroupName = vm.GroupName,
                TeacherId = vm.TeacherId,
                LanguageId = vm.LanguageId,
                StartDate = DateTime.SpecifyKind(vm.StartDate, DateTimeKind.Utc),
                LevelName = vm.LevelName
            };
            await _groupService.CreateGroupAsync(group);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            var vm = new GroupCreateViewModel { GroupName = group.GroupName, TeacherId = group.TeacherId, LanguageId = group.LanguageId, StartDate = group.StartDate, LevelName = group.LevelName };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, GroupCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
                ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
                return View(vm);
            }
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            group.GroupName = vm.GroupName;
            group.TeacherId = vm.TeacherId;
            group.LanguageId = vm.LanguageId;
            group.StartDate = DateTime.SpecifyKind(vm.StartDate, DateTimeKind.Utc);
            group.LevelName = vm.LevelName;
            await _groupService.UpdateGroupAsync(group);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _groupService.DeleteGroupAsync(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> SearchGroups(int? languageId, int? teacherId)
        {
            var groups = await _groupService.SearchGroupsAsync(languageId, teacherId);
            return PartialView("_GroupsPartial", groups);
        }
    }
}
