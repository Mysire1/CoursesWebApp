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

        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> Index()
        {
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        [Authorize(Roles = "Teacher,Student")]
        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();
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
                DateOfBirth = student.DateOfBirth.Kind == DateTimeKind.Utc ? student.DateOfBirth : DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc),
                Phone = student.Phone,
                Email = student.Email,
                GroupId = student.GroupId,
                HasDiscount = student.HasDiscount,
                DiscountPercentage = student.DiscountPercentage,
                PaymentStatus = student.PaymentStatus
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, StudentEditViewModel model)
        {
            if (id != model.StudentId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    var dbStudent = await _studentService.GetStudentByIdAsync(id);
                    if (dbStudent == null) return NotFound();
                    dbStudent.FirstName = model.FirstName;
                    dbStudent.LastName = model.LastName;
                    dbStudent.Email = model.Email;
                    dbStudent.Phone = model.Phone;
                    dbStudent.HasDiscount = model.HasDiscount;
                    dbStudent.DiscountPercentage = model.HasDiscount ? Math.Clamp(model.DiscountPercentage, 0, 100) : 0;
                    dbStudent.GroupId = model.GroupId;
                    dbStudent.PaymentStatus = model.PaymentStatus;
                    dbStudent.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc);
                    dbStudent.RegistrationDate = DateTime.SpecifyKind(dbStudent.RegistrationDate, DateTimeKind.Utc);
                    dbStudent.CreatedAt = DateTime.SpecifyKind(dbStudent.CreatedAt, DateTimeKind.Utc);
                    if(dbStudent.LastLoginAt.HasValue)
                        dbStudent.LastLoginAt = DateTime.SpecifyKind(dbStudent.LastLoginAt.Value, DateTimeKind.Utc);
                    await _studentService.UpdateStudentAsync(dbStudent);
                    TempData["SuccessMessage"] = "Студента оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message} | Inner: {(ex.InnerException?.Message ?? "немає")}");
                }
            }
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(model);
        }

        // ... збережено
    }
}
