using CoursesWebApp.Data;
using CoursesWebApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoursesWebApp.Services.Impl
{
    public class LevelServiceImpl : ILevelService
    {
        private readonly ApplicationDbContext _context;

        public LevelServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Level>> GetAllLevelsAsync()
        {
            return await _context.Levels
                .Include(l => l.Language)
                .ToListAsync();
        }

        public async Task<Level?> GetLevelByIdAsync(int id)
        {
            return await _context.Levels
                .Include(l => l.Language)
                .FirstOrDefaultAsync(l => l.LevelId == id);
        }
    }
}
