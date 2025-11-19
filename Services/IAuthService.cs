using CoursesWebApp.Models.ViewModels;

namespace CoursesWebApp.Services
{
    public interface IAuthService
    {
        Task<(object user, string role)?> ValidateUserAsync(string email, string password);
        Task<(object user, string role)?> RegisterUserAsync(RegisterViewModel model);
        Task<object?> GetUserByIdAsync(int id, string role);
        Task<object?> GetUserByEmailAsync(string email);
        Task<bool> IsEmailAvailableAsync(string email);
        Task UpdateStudentAsync(object student);
        Task UpdateTeacherAsync(object teacher);
        Task<bool> VerifyPasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}