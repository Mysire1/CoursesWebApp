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
                .Include(s => s.Group)
                .OrderBy(s => s.LastName)
                .ThenBy(s => s.FirstName)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.Students
                .Include(s => s.Group)
                .Include(s => s.Enrollments)
                .ThenInclude(e => e.Group)
                .ThenInclude(g => g.Level)
                .ThenInclude(l => l.Language)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }
        
        public async Task<Student?> FindByEmailAsync(string email)
        {
            return await _context.Students
                .Include(s => s.Group)
                .FirstOrDefaultAsync(s => s.Email == email);
        }

        public async Task<Student> CreateStudentAsync(Student student)
        {
            student.RegistrationDate = DateTime.UtcNow;
            student.CreatedAt = DateTime.UtcNow;
            if (student.DateOfBirth.Kind == DateTimeKind.Local)
                student.DateOfBirth = student.DateOfBirth.ToUniversalTime();
            else if(student.DateOfBirth.Kind == DateTimeKind.Unspecified)
                student.DateOfBirth = DateTime.SpecifyKind(student.DateOfBirth, DateTimeKind.Utc);
            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return student;
        }

        public async Task<Student> UpdateStudentAsync(Student student)
        {
            _context.Students.Update(student);
            await _context.SaveChangesAsync();
            return student;
        }
        
        public async Task UpdateStudentGroupAsync(int studentId, int? groupId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null)
                throw new Exception($"Студент з ID {studentId} не знайдено");
            student.GroupId = groupId;
            await _context.SaveChangesAsync();
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
                .Include(s => s.Group)
                .Where(s => s.HasDiscount)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsWithoutDiscountAsync()
        {
            return await _context.Students
                .Include(s => s.Group)
                .Where(s => !s.HasDiscount)
                .OrderBy(s => s.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Student>> GetStudentsLearningLanguageAsync(string languageName)
        {
            return await _context.Students
                .Include(s => s.Group)
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
                .Include(s => s.Group)
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
