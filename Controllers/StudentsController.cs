using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoursesWebApp.Controllers
{
    [Authorize(Roles = "Teacher")]
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
            var students = await _studentService.GetAllStudentsAsync();
            return View(students);
        }

        public async Task<IActionResult> Details(int id)
        {
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    var root = ex;
                    while (root.InnerException != null) root = root.InnerException;
                    ModelState.AddModelError("", $"Помилка при додаванні: {root.Message}");
                }
            }
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(student);
        }

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
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _studentService.UpdateStudentAsync(student);
                    TempData["SuccessMessage"] = "Студента оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message}");
                }
            }
            ViewBag.Groups = await _groupService.GetAllGroupsAsync();
            return View(student);
        }

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