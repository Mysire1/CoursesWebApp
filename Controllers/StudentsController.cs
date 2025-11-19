using CoursesWebApp.Models;
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
            {
                return NotFound();
            }
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(student);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
                return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var dbStudent = await _studentService.GetStudentByIdAsync(id);
                    if (dbStudent == null) return NotFound();
                    if (string.IsNullOrWhiteSpace(student.Email))
                    {
                        ModelState.AddModelError("Email", "Email не може бути порожнім!");
                        goto ErrorResult;
                    }
                    var another = await _studentService.FindByEmailAsync(student.Email);
                    if (another != null && another.StudentId != student.StudentId)
                    {
                        ModelState.AddModelError("Email", "Email вже використовується іншим студентом!");
                        goto ErrorResult;
                    }
                    dbStudent.FirstName = student.FirstName;
                    dbStudent.LastName = student.LastName;
                    dbStudent.Email = student.Email;
                    dbStudent.Phone = student.Phone;
                    dbStudent.HasDiscount = student.HasDiscount;
                    dbStudent.DiscountPercentage = student.HasDiscount ? Math.Clamp(student.DiscountPercentage, 0, 100) : 0;
                    dbStudent.GroupId = student.GroupId;
                    dbStudent.DateOfBirth = DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc);
                    dbStudent.RegistrationDate = DateTime.SpecifyKind(dbStudent.RegistrationDate, DateTimeKind.Utc);
                    dbStudent.CreatedAt = DateTime.SpecifyKind(dbStudent.CreatedAt, DateTimeKind.Utc);
                    if (dbStudent.LastLoginAt.HasValue)
                        dbStudent.LastLoginAt = DateTime.SpecifyKind(dbStudent.LastLoginAt.Value, DateTimeKind.Utc);
                    // ОЧИСТИТИ навігації для безпеки EF
                    dbStudent.Group = null;
                    dbStudent.Enrollments = null;
                    dbStudent.ExamResults = null;
                    dbStudent.Payments = null;
                    dbStudent.PaymentDeferrals = null;

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
            return View(student);
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
