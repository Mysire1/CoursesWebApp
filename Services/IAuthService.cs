using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;

namespace CoursesWebApp.Services
{
    public interface IAuthService
    {
        Task<User?> ValidateUserAsync(string username, string password);
        Task<User?> RegisterUserAsync(RegisterViewModel model);
        Task<User?> GetUserByIdAsync(int id);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> IsUsernameAvailableAsync(string username);
        Task<bool> IsEmailAvailableAsync(string email);
        Task UpdateUserAsync(User user);
        Task<bool> VerifyPasswordAsync(string password, string hash);
        string HashPassword(string password);
    }
}