using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Services.Impl
{
    public class TeacherServiceImpl : ITeacherService
    {
        private readonly ApplicationDbContext _context;

        public TeacherServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Teacher>> GetAllTeachersAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherLanguages)
                .ThenInclude(tl => tl.Language)
                .OrderBy(t => t.LastName)
                .ThenBy(t => t.FirstName)
                .ToListAsync();
        }

        public async Task<Teacher?> GetTeacherByIdAsync(int id)
        {
            return await _context.Teachers
                .Include(t => t.TeacherLanguages)
                .ThenInclude(tl => tl.Language)
                .Include(t => t.Groups)
                .ThenInclude(g => g.Level)
                .FirstOrDefaultAsync(t => t.TeacherId == id);
        }

        public async Task<IEnumerable<Teacher>> GetTeachersWithOneLanguageAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherLanguages)
                .ThenInclude(tl => tl.Language)
                .Where(t => t.TeacherLanguages.Count == 1)
                .OrderBy(t => t.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetTeachersWithTwoLanguagesAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherLanguages)
                .ThenInclude(tl => tl.Language)
                .Where(t => t.TeacherLanguages.Count == 2)
                .OrderBy(t => t.LastName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Teacher>> GetTeachersWithThreeLanguagesAsync()
        {
            return await _context.Teachers
                .Include(t => t.TeacherLanguages)
                .ThenInclude(tl => tl.Language)
                .Where(t => t.TeacherLanguages.Count >= 3)
                .OrderBy(t => t.LastName)
                .ToListAsync();
        }

        public async Task<Dictionary<int, List<Teacher>>> GetTeachersByLanguageCountAsync()
        {
            var teachers = await GetAllTeachersAsync();
            
            var result = new Dictionary<int, List<Teacher>>
            {
                { 1, new List<Teacher>() },
                { 2, new List<Teacher>() },
                { 3, new List<Teacher>() }
            };

            foreach (var teacher in teachers)
            {
                var languageCount = teacher.TeacherLanguages.Count;
                if (languageCount == 1)
                    result[1].Add(teacher);
                else if (languageCount == 2)
                    result[2].Add(teacher);
                else if (languageCount >= 3)
                    result[3].Add(teacher);
            }

            return result;
        }

        public async Task UpdateTeacherAsync(Teacher teacher)
        {
            _context.Teachers.Update(teacher);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTeacherLanguagesAsync(int teacherId, List<int> languageIds)
        {
            // Видаляємо всі старі зв’язки
            var existingLanguages = await _context.TeacherLanguages
                .Where(tl => tl.TeacherId == teacherId)
                .ToListAsync();
            
            _context.TeacherLanguages.RemoveRange(existingLanguages);
            
            // Додаємо нові зв’язки
            foreach (var languageId in languageIds)
            {
                _context.TeacherLanguages.Add(new TeacherLanguage
                {
                    TeacherId = teacherId,
                    LanguageId = languageId
                });
            }
            
            await _context.SaveChangesAsync();
        }
    }
}