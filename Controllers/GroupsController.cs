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
        private readonly IStudentService _studentService;

        public GroupsController(IGroupService groupService, ILanguageService languageService, ITeacherService teacherService, IStudentService studentService)
        {
            _groupService = groupService;
            _languageService = languageService;
            _teacherService = teacherService;
            _studentService = studentService;
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
            ViewBag.AllStudents = await _studentService.GetAllStudentsAsync();
            ViewBag.GroupId = id;
            
            var vm = new GroupCreateViewModel { 
                GroupName = group.GroupName, 
                TeacherId = group.TeacherId, 
                LanguageId = group.LanguageId, 
                StartDate = group.StartDate, 
                LevelName = group.LevelName 
            };
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
                ViewBag.AllStudents = await _studentService.GetAllStudentsAsync();
                ViewBag.GroupId = id;
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

        [HttpGet]
        public async Task<IActionResult> GetGroupStudents(int groupId)
        {
            var students = await _studentService.GetAllStudentsAsync();
            var groupStudents = students.Where(s => s.GroupId == groupId).ToList();
            return Json(groupStudents);
        }

        [HttpPost]
        public async Task<IActionResult> AddStudentToGroup([FromBody] AddStudentToGroupRequest request)
        {
            try
            {
                await _studentService.UpdateStudentGroupAsync(request.StudentId, request.GroupId);
                return Json(new { success = true, message = "Студента додано до групи" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Помилка: {ex.Message}" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveStudentFromGroup([FromBody] RemoveStudentFromGroupRequest request)
        {
            try
            {
                await _studentService.UpdateStudentGroupAsync(request.StudentId, null);
                return Json(new { success = true, message = "Студента видалено з групи" });
            }
            catch (Exception ex)
            {
            return Json(new { success = false, message = $"Помилка: {ex.Message}" });
            }
        }
    }
    
    public class AddStudentToGroupRequest
    {
        public int GroupId { get; set; }
        public int StudentId { get; set; }
    }
    
    public class RemoveStudentFromGroupRequest
    {
        public int StudentId { get; set; }
    }
}
