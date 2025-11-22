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
            return await _context.ExamResults
                .Include(er => er.Student)
                .Include(er => er.Exam)
                .ThenInclude(e => e.Level)
                .ThenInclude(l => l.Language)
                .Where(er => er.Grade < 60)
                .OrderBy(er => er.Student.LastName)
                .ThenBy(er => er.Student.FirstName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Level>> GetLevelsWithFailuresAsync()
        {
            return await _context.Levels
                .Include(l => l.Language)
                .Where(l => l.Exams.Any(e => e.ExamResults.Any(er => er.Grade < 60)))
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