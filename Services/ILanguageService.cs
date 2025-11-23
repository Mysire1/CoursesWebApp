using CoursesWebApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoursesWebApp.Services
{
    public interface ILanguageService
    {
        Task<IEnumerable<Language>> GetAllLanguagesAsync();
        Task<IEnumerable<LanguageWithGroupCount>> GetAllLanguagesWithGroupCountAsync();
        Task<Language?> GetLanguageByIdAsync(int id);
        Task<decimal> CalculateLanguageCostAsync(int languageId);
        Task<Dictionary<string, decimal>> CalculateAllLanguagesCostAsync();
        Task<Dictionary<string, decimal>> CalculateCostByLevelAsync(int languageId);
        Task<decimal> CalculateMonthlyLanguageCostAsync(int languageId);
    }

    public class LanguageWithGroupCount
    {
        public Language Language { get; set; } = null!;
        public int GroupCount { get; set; }
    }
}
