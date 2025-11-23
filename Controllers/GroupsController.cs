using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoursesWebApp.Data;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher,Student")]
    public class GroupsController : Controller
    {
        private readonly IGroupService _groupService;
        private readonly ILanguageService _languageService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;
        private readonly ApplicationDbContext _context;

        public GroupsController(IGroupService groupService, ILanguageService languageService, ITeacherService teacherService, IStudentService studentService, ApplicationDbContext context)
        {
            _groupService = groupService;
            _languageService = languageService;
            _teacherService = teacherService;
            _studentService = studentService;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
            ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(GroupCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
                ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
                return View(vm);
            }

            try
            {
                // Знайти або створити Level - ВИПРАВЛЕНО: Name замість LevelName
                var level = await _context.Levels
                    .FirstOrDefaultAsync(l => l.Name == vm.LevelName && l.LanguageId == vm.LanguageId);

                if (level == null)
                {
                    // Створити новий Level якщо не існує - ВИПРАВЛЕНО: Name замість LevelName
                    level = new Level
                    {
                        Name = vm.LevelName,
                        LanguageId = vm.LanguageId,
                        BaseCost = 1000m // Базова ціна за замовчуванням
                    };
                    _context.Levels.Add(level);
                    await _context.SaveChangesAsync();
                }

                var group = new Group
                {
                    GroupName = vm.GroupName,
                    TeacherId = vm.TeacherId,
                    LanguageId = vm.LanguageId,
                    LevelId = level.LevelId,
                    StartDate = DateTime.SpecifyKind(vm.StartDate, DateTimeKind.Utc),
                    LevelName = vm.LevelName
                };

                await _groupService.CreateGroupAsync(group);
                TempData["SuccessMessage"] = "Групу успішно створено!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Помилка при створенні групи: {ex.Message}");
                ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
                ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
                return View(vm);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
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
        [Authorize(Roles = "Teacher")]
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

            try
            {
                var group = await _groupService.GetGroupByIdAsync(id);
                if (group == null) return NotFound();

                // Знайти або створити Level - ВИПРАВЛЕНО: Name замість LevelName
                var level = await _context.Levels
                    .FirstOrDefaultAsync(l => l.Name == vm.LevelName && l.LanguageId == vm.LanguageId);

                if (level == null)
                {
                    level = new Level
                    {
                        Name = vm.LevelName,
                        LanguageId = vm.LanguageId,
                        BaseCost = 1000m
                    };
                    _context.Levels.Add(level);
                    await _context.SaveChangesAsync();
                }

                group.GroupName = vm.GroupName;
                group.TeacherId = vm.TeacherId;
                group.LanguageId = vm.LanguageId;
                group.LevelId = level.LevelId;
                group.StartDate = DateTime.SpecifyKind(vm.StartDate, DateTimeKind.Utc);
                group.LevelName = vm.LevelName;

                await _groupService.UpdateGroupAsync(group);
                TempData["SuccessMessage"] = "Групу успішно оновлено!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message}");
                ViewBag.Languages = await _languageService.GetAllLanguagesAsync();
                ViewBag.Teachers = await _teacherService.GetAllTeachersAsync();
                ViewBag.AllStudents = await _studentService.GetAllStudentsAsync();
                ViewBag.GroupId = id;
                return View(vm);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var group = await _groupService.GetGroupByIdAsync(id);
            if (group == null) return NotFound();
            return View(group);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _groupService.DeleteGroupAsync(id);
                TempData["SuccessMessage"] = "Групу успішно видалено!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка при видаленні: {ex.Message}";
            }
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
            var groupStudents = students
                .Where(s => s.GroupId == groupId)
                .Select(s => new {
                    s.StudentId,
                    s.FirstName,
                    s.LastName,
                    s.Phone
                }).ToList();
            return Json(groupStudents);
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
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
        [Authorize(Roles = "Teacher")]
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
