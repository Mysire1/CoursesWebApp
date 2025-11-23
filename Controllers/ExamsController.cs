using CoursesWebApp.Data;
using CoursesWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Controllers
{
    public class ExamsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ExamsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var exams = await _context.Exams
                .Include(e => e.Level)
                .ThenInclude(l => l.Language)
                .OrderByDescending(e => e.ExamDate)
                .ToListAsync();
            return View(exams);
        }

        public async Task<IActionResult> Details(int id)
        {
            var exam = await _context.Exams
                .Include(e => e.Level)
                .ThenInclude(l => l.Language)
                .FirstOrDefaultAsync(e => e.ExamId == id);
            if (exam == null)
                return NotFound();
            var results = await _context.ExamResults
                .Include(er => er.Student)
                .Where(er => er.ExamId == id)
                .ToListAsync();
            ViewBag.Exam = exam;
            return View(results);
        }
    }
}
