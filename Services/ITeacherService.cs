using CoursesWebApp.Models;

namespace CoursesWebApp.Services
{
    public interface ITeacherService
    {
        Task<IEnumerable<Teacher>> GetAllTeachersAsync();
        Task<Teacher?> GetTeacherByIdAsync(int id);
        Task<IEnumerable<Teacher>> GetTeachersWithOneLanguageAsync();
        Task<IEnumerable<Teacher>> GetTeachersWithTwoLanguagesAsync();
        Task<IEnumerable<Teacher>> GetTeachersWithThreeLanguagesAsync();
        Task<Dictionary<int, List<Teacher>>> GetTeachersByLanguageCountAsync();
        Task UpdateTeacherAsync(Teacher teacher);
        Task UpdateTeacherLanguagesAsync(int teacherId, List<int> languageIds);
    }
}