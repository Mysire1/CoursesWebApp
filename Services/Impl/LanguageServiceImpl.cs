using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Services;
using Microsoft.EntityFrameworkCore;

namespace CoursesWebApp.Services.Impl
{
    public class LanguageServiceImpl : ILanguageService
    {
        private readonly ApplicationDbContext _context;

        public LanguageServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LanguageWithGroupCount>> GetAllLanguagesWithGroupCountAsync()
        {
            var languages = await _context.Languages.OrderBy(l => l.Name).ToListAsync();
            var groupCounts = await _context.Groups.GroupBy(g => g.LanguageId)
                                     .Select(g => new { LanguageId = g.Key, Count = g.Count() }).ToListAsync();
            return languages.Select(l => new LanguageWithGroupCount {
                Language = l,
                GroupCount = groupCounts.FirstOrDefault(gc => gc.LanguageId == l.LanguageId)?.Count ?? 0
            }).ToList();
        }

        public async Task<IEnumerable<Language>> GetAllLanguagesAsync()
        {
            return await _context.Languages
                .OrderBy(l => l.Name)
                .ToListAsync();
        }

        public async Task<Language?> GetLanguageByIdAsync(int id)
        {
            return await _context.Languages
                .Include(l => l.Levels)
                .FirstOrDefaultAsync(l => l.LanguageId == id);
        }

        public async Task<decimal> CalculateLanguageCostAsync(int languageId)
        {
            var levels = await _context.Levels
                .Where(l => l.LanguageId == languageId)
                .ToListAsync();
            return levels.Sum(l => l.BaseCost);
        }

        public async Task<Dictionary<string, decimal>> CalculateAllLanguagesCostAsync()
        {
            var languages = await _context.Languages
                .Include(l => l.Levels)
                .ToListAsync();
            var costs = new Dictionary<string, decimal>();
            foreach (var language in languages)
            {
                costs[language.Name] = language.Levels.Sum(l => l.BaseCost);
            }
            return costs;
        }

        public async Task<Dictionary<string, decimal>> CalculateCostByLevelAsync(int languageId)
        {
            var levels = await _context.Levels
                .Where(l => l.LanguageId == languageId)
                .ToListAsync();
            var costs = new Dictionary<string, decimal>();
            foreach (var level in levels)
            {
                costs[level.Name] = level.BaseCost;
            }
            return costs;
        }

        public async Task<decimal> CalculateMonthlyLanguageCostAsync(int languageId)
        {
            var levels = await _context.Levels
                .Where(l => l.LanguageId == languageId)
                .ToListAsync();
            decimal totalCost = levels.Sum(l => l.BaseCost);
            int totalMonths = levels.Sum(l => l.DurationMonths);
            return totalMonths > 0 ? totalCost / totalMonths : 0;
        }
    }

    public class LanguageWithGroupCount
    {
        public Language Language { get; set; } = null!;
        public int GroupCount { get; set; }
    }
}
