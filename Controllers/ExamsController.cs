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

        // GET: Exams
        public async Task<IActionResult> Index()
        {
            try
            {
                var exams = await _context.Exams
                    .Include(e => e.ExamResults)
                    .OrderByDescending(e => e.ExamDate)
                    .ToListAsync();
                return View(exams);
            }
            catch (Exception ex)
            {
                ViewBag.ExamError = ex.ToString();
                return View("ErrorInExams");
            }
        }

        // GET: Exams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.ExamResults)
                    .ThenInclude(er => er.Student)
                .FirstOrDefaultAsync(m => m.ExamId == id);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        public IActionResult Create()
        {
            ViewBag.Levels = ExamLevels;
            ViewBag.Students = _context.Students.Where(s => s.IsActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,ExamDate,Level")] Exam exam, int[] SelectedStudentIds)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.Exams.Add(exam);
                    await _context.SaveChangesAsync();
                    if (SelectedStudentIds != null && SelectedStudentIds.Length > 0)
                    {
                        foreach (var studentId in SelectedStudentIds)
                        {
                            var examResult = new ExamResult
                            {
                                ExamId = exam.ExamId,
                                StudentId = studentId,
                                Grade = 0,
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
                ModelState.AddModelError("", ex.ToString());
            }
            ViewBag.Levels = ExamLevels;
            ViewBag.Students = _context.Students.Where(s => s.IsActive).OrderBy(s => s.LastName).ThenBy(s => s.FirstName).ToList();
            return View(exam);
        }

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
            ViewBag.Levels = ExamLevels;
            return View(exam);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExamId,Description,ExamDate,Level")] Exam exam)
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
                    if (!_context.Exams.Any(e => e.ExamId == id))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.ToString());
                }
            }
            ViewBag.Levels = ExamLevels;
            return View(exam);
        }

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
        public async Task<IActionResult> EditGrade(int id, [Bind("ExamResultId,ExamId,StudentId,Grade,ExamDate")] ExamResult examResult)
        {
            if (id != examResult.ExamResultId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(examResult);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Оцінку успішно оновлено!";
                    return RedirectToAction(nameof(Details), new { id = examResult.ExamId });
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ExamResults.Any(e => e.ExamResultId == id))
                    {
                        return NotFound();
                    }
                    throw;
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

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var exam = await _context.Exams
                .Include(e => e.ExamResults)
                .FirstOrDefaultAsync(m => m.ExamId == id);

            if (exam == null)
            {
                return NotFound();
            }

            return View(exam);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var exam = await _context.Exams.FindAsync(id);
            if (exam != null)
            {
                _context.Exams.Remove(exam);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Екзамен успішно видалено!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}