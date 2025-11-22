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
        private readonly ApplicationDbContext _context;

        public StudentsController(IStudentService studentService, IGroupService groupService, ApplicationDbContext context)
        {
            _studentService = studentService;
            _groupService = groupService;
            _context = context;
        }

        // ...інші методи залишаються без змін...

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Pay(int id, decimal amount)
        {
            if (amount <= 0)
            {
                TempData["ErrorMessage"] = "Сума має бути більшою за нуль.";
                return RedirectToAction("Details", new { id });
            }
            var student = await _studentService.GetStudentByIdAsync(id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Студент не знайдений.";
                return RedirectToAction("Index");
            }

            // Додаємо оплату
            var payment = new Payment
            {
                StudentId = student.StudentId,
                Amount = amount,
                PaidAt = DateTime.UtcNow
            };
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Оплата {amount:C} успішно додана.";
            return RedirectToAction("Details", new { id });
        }
    }
}
