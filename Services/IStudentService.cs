using CoursesWebApp.Models;

namespace CoursesWebApp.Services
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<Student?> FindByEmailAsync(string email);
        Task<Student> CreateStudentAsync(Student student);
        Task<Student> UpdateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<IEnumerable<Student>> GetStudentsWithDiscountAsync();
        Task<IEnumerable<Student>> GetStudentsWithoutDiscountAsync();
        Task<IEnumerable<Student>> GetStudentsLearningLanguageAsync(string languageName);
        Task<IEnumerable<Student>> GetStudentsLearningMultipleLanguagesAsync();
        Task<int> ApplyLoyaltyDiscountAsync();
        Task UpdateStudentGroupAsync(int studentId, int? groupId);
    }
}
