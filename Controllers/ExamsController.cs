using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoursesWebApp.Data;
using CoursesWebApp.Models;

namespace CoursesWebApp.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Include(e => e.Level)
                    .ThenInclude(l => l.Language)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
            return View(exams);
        }

        // GET: Exams/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Levels = await _context.Levels
                .Include(l => l.Language)
                .OrderBy(l => l.Language.Name)
                .ThenBy(l => l.LevelName)
                .ToListAsync();
            
            ViewBag.Students = await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
            
            return View();
        }

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,ExamDate,LevelId")] Exam exam, int[] SelectedStudentIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Додаємо екзамен до контексту
                    _context.Exams.Add(exam);
                    await _context.SaveChangesAsync();

                    // Створюємо записи результатів екзамену для вибраних студентів
                    if (SelectedStudentIds != null && SelectedStudentIds.Length > 0)
                    {
                        foreach (var studentId in SelectedStudentIds)
                        {
                            var examResult = new ExamResult
                            {
                                ExamId = exam.ExamId,
                                StudentId = studentId,
                                Grade = 0, // Оцінка буде додана пізніше
                                ExamDate = exam.ExamDate
                            };
                            _context.ExamResults.Add(examResult);
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Екзамен успішно створено!";
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Помилка при збереженні екзамена: {ex.Message}");
            }

            // Якщо виникла помилка, повертаємо форму з даними
            ViewBag.Levels = await _context.Levels
                .Include(l => l.Language)
                .OrderBy(l => l.Language.Name)
                .ThenBy(l => l.LevelName)
                .ToListAsync();
            
            ViewBag.Students = await _context.Students
                .Where(s => s.IsActive)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
            
            return View(exam);
        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.Level)
                    .ThenInclude(l => l.Language)
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Student)
                .FirstOrDefaultAsync(m => m.ExamId == id);
            
            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        // GET: Exams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams.FindAsync(id);
            if (exam == null)
            {
                return NotFound();
            }

            ViewBag.Levels = await _context.Levels
                .Include(l => l.Language)
                .OrderBy(l => l.Language.Name)
                .ThenBy(l => l.LevelName)
                .ToListAsync();

            return View(exam);
        }

        // POST: Exams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExamId,Description,ExamDate,LevelId")] Exam exam)
        {
            if (id != exam.ExamId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exam);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Екзамен успішно оновлено!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExamExists(exam.ExamId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Помилка при оновленні: {ex.Message}");
                }
            }

            ViewBag.Levels = await _context.Levels
                .Include(l => l.Language)
                .OrderBy(l => l.Language.Name)
                .ThenBy(l => l.LevelName)
                .ToListAsync();

            return View(exam);
        }

        // POST: Exams/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var exam = await _context.Exams.FindAsync(id);
                if (exam == null)
                {
                    return Json(new { success = false, message = "Екзамен не знайдено" });
                }

                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Екзамен успішно видалено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Помилка: {ex.Message}" });
            }
        }

        // POST: Exams/UpdateGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateGrade(int examResultId, int grade)
        {
            try
            {
                var examResult = await _context.ExamResults.FindAsync(examResultId);
                if (examResult == null)
                {
                    return Json(new { success = false, message = "Результат екзамену не знайдено" });
                }

                examResult.Grade = grade;
                await _context.SaveChangesAsync();
                
                return Json(new { success = true, message = "Оцінку оновлено" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Помилка: {ex.Message}" });
            }
        }

        private bool ExamExists(int id)
        {
            return _context.Exams.Any(e => e.ExamId == id);
        }
    }
}