using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher,Student")]
    public class StudentsController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;

        public StudentsController(IStudentService studentService, IGroupService groupService)
        {
            _studentService = studentService;
            _groupService = groupService;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Student"))
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                {
                    TempData["ErrorMessage"] = "Профіль студента не знайдено. Зверніться до адміністратора.";
                    return View(new List<Student>());
                }
                var student = await _studentService.FindByEmailAsync(email);
                if (student == null)
                {
                    TempData["ErrorMessage"] = "Профіль студента не знайдено. Зверніться до адміністратора.";
                    return View(new List<Student>());
                }
                return View(new List<Student> { student });
            }
            else
            {
                var students = await _studentService.GetAllStudentsAsync();
                return View(students);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null) return NotFound();
            if (User.IsInRole("Student"))
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (email != student.Email)
                    return Forbid();
            }
            return View(student);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(Student student)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.CreateStudentAsync(student);
                    TempData["SuccessMessage"] = $"Студента {student.FullName} успішно додано!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при додаванні: {ex.Message}");
                }
            }
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(student);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            var model = new StudentEditViewModel
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth.ToUniversalTime(),
                Phone = student.Phone,
                Email = student.Email,
                GroupId = student.GroupId,
                HasDiscount = student.HasDiscount,
                DiscountPercentage = student.DiscountPercentage
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, StudentEditViewModel model)
        {
            if (id != model.StudentId)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var dbStudent = await _studentService.GetStudentByIdAsync(id);
                    if (dbStudent == null) return NotFound();
                    if (string.IsNullOrWhiteSpace(model.Email))
                    {
                        ModelState.AddModelError("Email", "Email не може бути порожнім!");
                        goto ErrorResult;
                    }
                    var another = await _studentService.FindByEmailAsync(model.Email);
                    if (another != null && another.StudentId != model.StudentId)
                    {
                        ModelState.AddModelError("Email", "Email вже використовується іншим студентом!");
                        goto ErrorResult;
                    }
                    dbStudent.FirstName = model.FirstName;
                    dbStudent.LastName = model.LastName;
                    dbStudent.Email = model.Email;
                    dbStudent.Phone = model.Phone;
                    dbStudent.HasDiscount = model.HasDiscount;
                    dbStudent.DiscountPercentage = model.HasDiscount ? Math.Clamp(model.DiscountPercentage, 0, 100) : 0;
                    dbStudent.GroupId = model.GroupId;
                    dbStudent.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc).ToUniversalTime();
                    await _studentService.UpdateStudentAsync(dbStudent);
                    TempData["SuccessMessage"] = "Студента оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message} | Inner: {(ex.InnerException?.Message ?? "немає")}");
                }
            }
            ErrorResult:
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(model);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var result = await _studentService.DeleteStudentAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Студента видалено!";
            }
            else
            {
                TempData["ErrorMessage"] = "Помилка при видаленні студента.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> ApplyLoyaltyDiscount()
        {
            try
            {
                int count = await _studentService.ApplyLoyaltyDiscountAsync();
                TempData["SuccessMessage"] = $"Знижку надано {count} студентам!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
