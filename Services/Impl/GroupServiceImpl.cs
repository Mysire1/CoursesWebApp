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
                .Include(g => g.Enrollments)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        public async Task<Group?> GetGroupByIdAsync(int id)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(g => g.Teacher)
                .Include(g => g.Enrollments)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(g => g.GroupId == id);
        }

        public async Task<IEnumerable<Group>> SearchGroupsAsync(int? languageId = null, int? teacherId = null)
        {
            var query = _context.Groups
                .Include(g => g.Level)
                .ThenInclude(l => l.Language)
                .Include(g => g.Teacher)
                .Include(g => g.Enrollments)
                .AsQueryable();

            if (languageId.HasValue)
            {
                query = query.Where(g => g.LanguageId == languageId.Value);
            }

            if (teacherId.HasValue)
            {
                query = query.Where(g => g.TeacherId == teacherId.Value);
            }

            return await query
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetSmallGroupsAsync(int maxStudents = 5)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .Include(g => g.Teacher)
                .Include(g => g.Enrollments)
                .Where(g => g.Enrollments.Count < maxStudents)
                .OrderBy(g => g.Enrollments.Count)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetLargeGroupsAsync(int exactStudents = 20)
        {
            return await _context.Groups
                .Include(g => g.Level)
                .Include(g => g.Teacher)
                .Include(g => g.Enrollments)
                .Where(g => g.Enrollments.Count == exactStudents)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        public async Task<int> ApplySmallGroupSurchargeAsync(decimal surchargePercentage = 20)
        {
            var smallGroups = await GetSmallGroupsAsync();
            int count = 0;

            foreach (var group in smallGroups)
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.GroupId == group.GroupId)
                    .ToListAsync();

                foreach (var enrollment in enrollments)
                {
                    enrollment.Cost *= (1 + surchargePercentage / 100);
                }
                count += enrollments.Count;
            }

            await _context.SaveChangesAsync();
            return count;
        }

        public async Task<int> ApplyLargeGroupDiscountAsync(decimal discountPercentage = 5)
        {
            var largeGroups = await GetLargeGroupsAsync();
            int count = 0;

            foreach (var group in largeGroups)
            {
                var enrollments = await _context.Enrollments
                    .Where(e => e.GroupId == group.GroupId)
                    .ToListAsync();

                foreach (var enrollment in enrollments)
                {
                    enrollment.Cost *= (1 - discountPercentage / 100);
                }
                count += enrollments.Count;
            }

            await _context.SaveChangesAsync();
            return count;
        }

        public async Task CreateGroupAsync(Group group)
        {
            _context.Groups.Add(group);
            await _context.SaveChangesAsync();
        }
    }
}
