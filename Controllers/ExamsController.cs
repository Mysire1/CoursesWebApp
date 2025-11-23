using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesWebApp.Data;
using CoursesWebApp.Models;

namespace CoursesWebApp.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly List<string> ExamLevels = new()
        {
            "Beginner (A1)",
            "Elementary (A2)",
            "Intermediate (B1)",
            "Upper-Intermediate (B2)",
            "Advanced (C1)"
        };

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ... (інші action-и не змінені)

        // GET: Exams/EditGrade/5
        public async Task<IActionResult> EditGrade(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var examResult = await _context.ExamResults
                .Include(er => er.Student)
                .Include(er => er.Exam)
                .FirstOrDefaultAsync(er => er.ExamResultId == id);

            if (examResult == null)
            {
                return NotFound();
            }

            return View(examResult);
        }

        // POST: Exams/EditGrade/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(int id, [Bind("ExamResultId,Grade")] ExamResult examResult)
        {
            if (id != examResult.ExamResultId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var dbResult = await _context.ExamResults.FindAsync(id);
                    if (dbResult == null)
                        return NotFound();

                    dbResult.Grade = examResult.Grade;
                    _context.Entry(dbResult).Property(x => x.Grade).IsModified = true;
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Оцінку успішно оновлено!";
                    return RedirectToAction(nameof(Details), new { id = dbResult.ExamId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
            }
            examResult = await _context.ExamResults
                .Include(er => er.Student)
                .Include(er => er.Exam)
                .FirstOrDefaultAsync(er => er.ExamResultId == id);
            return View(examResult);
        }

        // ... (інші action-и не змінені)
    }
}
