using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Services.Impl
{
    public class QueryServiceImpl : IQueryService
    {
        private readonly ApplicationDbContext _context;

        public QueryServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamResult>> GetFailedExamResultsAsync()
        {
            // Прибираємо .ThenInclude(e => e.Level).ThenInclude(l => l.Language)
            return await _context.ExamResults
                .Include(er => er.Student)
                .Include(er => er.Exam)
                .Where(er => er.Grade < 60)
                .OrderBy(er => er.Student.LastName)
                .ThenBy(er => er.Student.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Level>> GetLevelsWithFailuresAsync()
        {
            // КРОК 1: Отримуємо список НАЗВ рівнів (рядків), де були провалені іспити.
            // Ми беремо дані з ExamResults -> Exam -> Level (це рядок, згідно з вашим скріншотом)
            var failedLevelNames = await _context.ExamResults
                .Include(er => er.Exam)
                .Where(er => er.Grade < 60)
                .Select(er => er.Exam.Level) // Беремо поле Level (string), а не LevelId
                .Distinct()
                .ToListAsync();

            // КРОК 2: Шукаємо у таблиці Levels записи, у яких Name співпадає зі знайденими рядками.
            return await _context.Levels
                .Include(l => l.Language)
                // Припускаємо, що у моделі Level є властивість Name. 
                // Якщо вона називається інакше (напр. Title), замініть l.Name на правильну назву.
                .Where(l => failedLevelNames.Contains(l.Name)) 
                .OrderBy(l => l.Language.Name)
                .ThenBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<int> GetFailedExamsCountAsync()
        {
            return await _context.ExamResults
                .CountAsync(er => er.Grade < 60);
        }

        public async Task<IEnumerable<Student>> GetFullyPaidStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Payments)
                .Where(s => s.Payments.Sum(p => p.Amount) >= s.Enrollments.Sum(e => e.Cost))
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetNotFullyPaidStudentsAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Payments)
                .Where(s => s.Payments.Sum(p => p.Amount) < s.Enrollments.Sum(e => e.Cost))
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsWithDebtLessThan50Async()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .Include(s => s.Payments)
                .Where(s => s.Enrollments.Sum(e => e.Cost) > 0 &&
                           (s.Payments.Sum(p => p.Amount) / s.Enrollments.Sum(e => e.Cost)) > 0.5m)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsWithDeferralsAsync()
        {
            return await _context.Students
                .Include(s => s.PaymentDeferrals)
                .Where(s => s.PaymentDeferrals.Any(pd => !pd.IsPaid))
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetScheduleByGroupAsync(int groupId)
        {
            return await _context.Schedules
                .Include(s => s.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(s => s.Group)
                .ThenInclude(g => g.Teacher)
                .Include(s => s.Group)
                .ThenInclude(g => g.Language)
                .Include(s => s.Group)
                .ThenInclude(g => g.Students)
                .Include(s => s.Group)
                .ThenInclude(g => g.Enrollments)
                .Include(s => s.Classroom)
                .Where(s => s.GroupId == groupId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Schedule>> GetScheduleByTeacherAsync(int teacherId)
        {
            return await _context.Schedules
                .Include(s => s.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(s => s.Group)
                .ThenInclude(g => g.Teacher)
                .Include(s => s.Group)
                .ThenInclude(g => g.Language)
                .Include(s => s.Group)
                .ThenInclude(g => g.Students)
                .Include(s => s.Group)
                .ThenInclude(g => g.Enrollments)
                .Include(s => s.Classroom)
                .Where(s => s.Group.TeacherId == teacherId)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();
        }
    }
}