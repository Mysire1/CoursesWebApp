using CoursesWebApp.Models;
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
        private readonly ILanguageService _languageService;
        
        public TeachersController(ITeacherService teacherService, ILanguageService languageService)
        {
            _teacherService = teacherService;
            _languageService = languageService;
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var teacher = await _teacherService.GetTeacherByIdAsync(id);
                if (teacher == null)
                {
                    TempData["ErrorMessage"] = "Викладача не знайдено";
                    return RedirectToAction(nameof(Index));
                }
                
                // Завантажуємо всі мови для вибору
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                ViewBag.AllLanguages = allLanguages;
                
                // ID мов, які викладач вже викладає
                ViewBag.SelectedLanguageIds = teacher.TeacherLanguages?.Select(tl => tl.LanguageId).ToList() ?? new List<int>();
                
                return View(teacher);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Помилка: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher model, List<int> selectedLanguageIds)
        {
            if (id != model.TeacherId)
            {
                TempData["ErrorMessage"] = "Невірний ID викладача";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var existingTeacher = await _teacherService.GetTeacherByIdAsync(id);
                if (existingTeacher == null)
                {
                    TempData["ErrorMessage"] = "Викладача не знайдено";
                    return RedirectToAction(nameof(Index));
                }

                // Оновлюємо тільки необхідні поля
                existingTeacher.FirstName = model.FirstName;
                existingTeacher.LastName = model.LastName;
                existingTeacher.Email = model.Email;
                existingTeacher.Phone = model.Phone;
                existingTeacher.IsActive = model.IsActive;
                
                // Забезпечуємо UTC для дат
                if (existingTeacher.HireDate.Kind != DateTimeKind.Utc)
                    existingTeacher.HireDate = DateTime.SpecifyKind(existingTeacher.HireDate, DateTimeKind.Utc);
                
                if (existingTeacher.LastLoginAt.HasValue && existingTeacher.LastLoginAt.Value.Kind != DateTimeKind.Utc)
                    existingTeacher.LastLoginAt = DateTime.SpecifyKind(existingTeacher.LastLoginAt.Value, DateTimeKind.Utc);

                await _teacherService.UpdateTeacherAsync(existingTeacher);
                
                // Оновлюємо мови викладача
                await _teacherService.UpdateTeacherLanguagesAsync(id, selectedLanguageIds ?? new List<int>());
                
                TempData["SuccessMessage"] = "Викладача успішно оновлено!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var errorMessage = $"Помилка при оновленні: {ex.Message}";
                if (ex.InnerException != null)
                {
                    errorMessage += $" | Деталі: {ex.InnerException.Message}";
                }
                TempData["ErrorMessage"] = errorMessage;
                
                // Перезавантажуємо дані для повторного відображення
                var allLanguages = await _languageService.GetAllLanguagesAsync();
                ViewBag.AllLanguages = allLanguages;
                ViewBag.SelectedLanguageIds = selectedLanguageIds ?? new List<int>();
                
                return View(model);
            }
        }
    }
}
