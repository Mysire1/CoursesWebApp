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

        // ... інші методи залишаються як є ...

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
                return NotFound();
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();

            // Мапінг Student -> StudentEditViewModel
            var model = new StudentEditViewModel
            {
                StudentId = student.StudentId,
                FirstName = student.FirstName,
                LastName = student.LastName,
                DateOfBirth = student.DateOfBirth,
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
                    // Email унікальність
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
                    // Копіюємо лише ті поля, які редагуються
                    dbStudent.FirstName = model.FirstName;
                    dbStudent.LastName = model.LastName;
                    dbStudent.Email = model.Email;
                    dbStudent.Phone = model.Phone;
                    dbStudent.HasDiscount = model.HasDiscount;
                    dbStudent.DiscountPercentage = model.HasDiscount ? Math.Clamp(model.DiscountPercentage, 0, 100) : 0;
                    dbStudent.GroupId = model.GroupId;
                    dbStudent.DateOfBirth = DateTime.SpecifyKind(model.DateOfBirth, DateTimeKind.Utc);
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
    }
}
