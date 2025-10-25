using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Services.Impl
{
    public class StudentServiceImpl : IStudentService
    {
        private readonly ApplicationDbContext _context;

        public StudentServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.Students
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            student.RegistrationDate = DateTime.Now;
            student.CreatedAt = DateTime.Now;
            
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            _context.Entry(student).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return false;

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Student>> GetStudentsWithDiscountAsync()
        {
            return await _context.Students
                .Where(s => s.HasDiscount)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsWithoutDiscountAsync()
        {
            return await _context.Students
                .Where(s => !s.HasDiscount)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsLearningLanguageAsync(string languageName)
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .Where(s => s.Enrollments.Any(e => e.Group.Level.Language.Name == languageName))
                .Distinct()
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsLearningMultipleLanguagesAsync()
        {
            return await _context.Students
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .Where(s => s.Enrollments
                    .Select(e => e.Group.Level.LanguageId)
                    .Distinct()
                    .Count() > 1)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<int> ApplyLoyaltyDiscountAsync()
        {
            // Знайти студентів, які завершили більше 3-х рівнів і не мають знижки
            var eligibleStudents = await _context.Students
                .Include(s => s.Enrollments)
                .Where(s => !s.HasDiscount && 
                           s.Enrollments.Count(e => e.IsCompleted) > 3)
                .ToListAsync();

            foreach (var student in eligibleStudents)
            {
                student.HasDiscount = true;
                student.DiscountPercentage = 10;
            }

            await _context.SaveChangesAsync();
            return eligibleStudents.Count;
        }
    }
}