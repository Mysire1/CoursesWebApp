using CoursesWebApp.Data;
using CoursesWebApp.Models;
using CoursesWebApp.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace CoursesWebApp.Services.Impl
{
    public class AuthServiceImpl : IAuthService
    {
        private readonly ApplicationDbContext _context;

        public AuthServiceImpl(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> ValidateUserAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => (u.Username == username || u.Email == username) && u.IsActive);

            if (user != null && VerifyPassword(password, user.PasswordHash))
            {
                user.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return user;
            }

            return null;
        }

        public async Task<User?> RegisterUserAsync(RegisterViewModel model)
        {
            if (!await IsUsernameAvailableAsync(model.Username) || !await IsEmailAvailableAsync(model.Email))
            {
                return null;
            }

            var user = new User
            {
                Username = model.Username,
                Email = model.Email,
                PasswordHash = HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                Role = model.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Якщо реєструється студент, створюємо запис в таблиці Students
            if (model.Role == "Student")
            {
                try
                {
                    var student = new Student
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = "",
                        DateOfBirth = DateTime.SpecifyKind(DateTime.Now.AddYears(-20), DateTimeKind.Utc), // За замовчуванням
                        RegistrationDate = DateTime.UtcNow,
                        HasDiscount = false,
                        DiscountPercentage = 0,
                        CreatedAt = DateTime.UtcNow,
                        Status = "Активний"
                    };

                    _context.Students.Add(student);
                    await _context.SaveChangesAsync();

                    user.StudentId = student.StudentId;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    // Логуємо помилку, але не падаємо — користувач уже створений в Identity
                    Console.WriteLine($"Помилка створення студента: {ex.Message}");
                }
            }
            else if (model.Role == "Teacher")
            {
                try
                {
                    var teacher = new Teacher
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        Phone = "",
                        HireDate = DateTime.UtcNow
                    };

                    _context.Teachers.Add(teacher);
                    await _context.SaveChangesAsync();

                    user.TeacherId = teacher.TeacherId;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка створення викладача: {ex.Message}");
                }
            }

            return user;
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .Include(u => u.Student)
                .Include(u => u.Teacher)
                .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
        }

        public async Task<bool> IsUsernameAvailableAsync(string username)
        {
            return !await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            return !await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> VerifyPasswordAsync(string password, string hash)
        {
            return await Task.FromResult(VerifyPassword(password, hash));
        }

        public string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password + "CoursesWebAppSalt"));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            var passwordHash = HashPassword(password);
            return passwordHash == hash;
        }
    }
}