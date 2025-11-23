using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Services.Impl
{
    public class GroupServiceImpl : IGroupService
    {
        private readonly ApplicationDbContext _context;

        public GroupServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Group>> GetAllGroupsAsync()
        {
            return await _context.Groups
                .Include(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(g => g.Teacher)
                .Include(g => g.Language)
                .Include(g => g.Students)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(g => g.Teacher)
                .Include(g => g.Language)
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.GroupId == id);
        }

        public async Task<IEnumerable<Group>> SearchGroupsAsync(int? languageId = null, int? teacherId = null)
        {
            var query = _context.Groups
                .Include(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(g => g.Teacher)
                .Include(g => g.Language)
                .Include(g => g.Students)
                .AsQueryable();

            if (languageId.HasValue)
            {
                query = query.Where(g => g.LanguageId == languageId.Value);
            }
            if (teacherId.HasValue)
            {
                query = query.Where(g => g.TeacherId == teacherId.Value);
            }

            return await query.OrderBy(g => g.GroupName).ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetSmallGroupsAsync(int maxStudents = 5)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .Include(g => g.Teacher)
                .Include(g => g.Students)
                .Where(g => g.Students.Count < maxStudents)
                .OrderBy(g => g.Students.Count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetLargeGroupsAsync(int exactStudents = 20)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .Include(g => g.Teacher)
                .Include(g => g.Students)
                .Where(g => g.Students.Count == exactStudents)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        // Надбавка для малих груп (<5) - тільки після кліку
        public async Task<int> ApplySmallGroupSurchargeAsync(decimal surchargePercentage = 20)
        {
            var smallGroups = await GetSmallGroupsAsync();
            int count = 0;

            foreach (var group in smallGroups)
            {
                var students = await _context.Students
                    .Where(s => s.GroupId == group.GroupId)
                    .ToListAsync();

                foreach (var student in students)
                {
                    student.PaymentStatus = "Надбавка";
                    student.DiscountPercentage = -20; // +20% надбавка
                }
                count += students.Count;
            }

            await _context.SaveChangesAsync();
            return count;
        }

        // Знижка для великих груп (=20) - тільки після кліку
        public async Task<int> ApplyLargeGroupDiscountAsync(decimal discountPercentage = 5)
        {
            var largeGroups = await GetLargeGroupsAsync();
            int count = 0;

            foreach (var group in largeGroups)
            {
                var students = await _context.Students
                    .Where(s => s.GroupId == group.GroupId)
                    .ToListAsync();

                foreach (var student in students)
                {
                    student.PaymentStatus = "Знижка";
                    student.DiscountPercentage = 5; // 5% знижка
                }
                count += students.Count;
            }

            await _context.SaveChangesAsync();
            return count;
        }

        public async Task CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateGroupAsync(Group group)
        {
            _context.Groups.Update(group);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteGroupAsync(int id)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group != null)
            {
                _context.Groups.Remove(group);
                await _context.SaveChangesAsync();
            }
        }
    }
}
