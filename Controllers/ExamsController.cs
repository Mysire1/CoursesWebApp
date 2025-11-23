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
            ViewBag.Levels = await _context.Levels.Include(l => l.Language).ToListAsync();
            ViewBag.Students = await _context.Students.Where(s => s.IsActive).ToListAsync();
            return View();
        }

        // POST: Exams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,ExamDate,LevelId")] Exam exam, int[] SelectedStudentIds)
        {
            if (ModelState.IsValid)
            {
                _context.Exams.Add(exam);
                await _context.SaveChangesAsync();
                // Create empty ExamResult records for selected students
                foreach (var studentId in SelectedStudentIds)
                {
                    var result = new ExamResult
                    {
                        ExamId = exam.ExamId,
                        StudentId = studentId,
                        Grade = 0, // will be updated later
                        ExamDate = exam.ExamDate
                    };
                    _context.ExamResults.Add(result);
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Levels = await _context.Levels.Include(l => l.Language).ToListAsync();
            ViewBag.Students = await _context.Students.Where(s => s.IsActive).ToListAsync();
            return View(exam);
        }

        // Other actions...
    }
}
