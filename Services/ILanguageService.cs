using CoursesWebApp.Models;

namespace CoursesWebApp.Services
{
    public interface ILanguageService
    {
        Task<IEnumerable<Language>> GetAllLanguagesAsync();
        Task<Language?> GetLanguageByIdAsync(int id);
        Task<decimal> CalculateLanguageCostAsync(int languageId);
        Task<Dictionary<string, decimal>> CalculateAllLanguagesCostAsync();
        Task<Dictionary<string, decimal>> CalculateCostByLevelAsync(int languageId);
        Task<decimal> CalculateMonthlyLanguageCostAsync(int languageId);
    }
}