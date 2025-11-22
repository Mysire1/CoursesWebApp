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

        public async Task<(object user, string role)?> ValidateUserAsync(string email, string password)
        {
            // Спочатку шукаємо в Students
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email && s.IsActive);

            if (student != null && VerifyPassword(password, student.PasswordHash))
            {
                student.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (student, "Student");
            }

            // Якщо не знайшли студента, шукаємо в Teachers
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == email && t.IsActive);

            if (teacher != null && VerifyPassword(password, teacher.PasswordHash))
            {
                teacher.LastLoginAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                return (teacher, "Teacher");
            }

            return null;
        }

        public async Task<(object user, string role)?> RegisterUserAsync(RegisterViewModel model)
        {
            if (!await IsEmailAvailableAsync(model.Email))
            {
                return null;
            }

            if (model.Role == "Student")
            {
                var student = new Student
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Phone = model.Phone,
                    DateOfBirth = DateTime.SpecifyKind(DateTime.Now.AddYears(-20), DateTimeKind.Utc),
                    RegistrationDate = DateTime.UtcNow,
                    HasDiscount = false,
                    DiscountPercentage = 0,
                    CreatedAt = DateTime.UtcNow,
                    PaymentStatus = "Активний",
                    IsActive = true
                };

                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return (student, "Student");
            }
            else if (model.Role == "Teacher")
            {
                var teacher = new Teacher
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PasswordHash = HashPassword(model.Password),
                    Phone = model.Phone,
                    HireDate = DateTime.UtcNow,
                    IsActive = true
                };

                _context.Teachers.Add(teacher);
                await _context.SaveChangesAsync();
                return (teacher, "Teacher");
            }

            return null;
        }

        public async Task<object?> GetUserByIdAsync(int id, string role)
        {
            if (role == "Student")
            {
                return await _context.Students
                    .Include(s => s.Group)
                    .Include(s => s.Enrollments)
                    .FirstOrDefaultAsync(s => s.StudentId == id && s.IsActive);
            }
            else if (role == "Teacher")
            {
                return await _context.Teachers
                    .Include(t => t.Groups)
                    .Include(t => t.TeacherLanguages)
                    .FirstOrDefaultAsync(t => t.TeacherId == id && t.IsActive);
            }

            return null;
        }

        public async Task<object?> GetUserByEmailAsync(string email)
        {
            // Спочатку шукаємо в Students
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == email && s.IsActive);

            if (student != null)
            {
                return student;
            }

            // Якщо не знайшли, шукаємо в Teachers
            var teacher = await _context.Teachers
                .FirstOrDefaultAsync(t => t.Email == email && t.IsActive);

            return teacher;
        }

        public async Task<bool> IsEmailAvailableAsync(string email)
        {
            var studentExists = await _context.Students.AnyAsync(s => s.Email == email);
            if (studentExists) return false;

            var teacherExists = await _context.Teachers.AnyAsync(t => t.Email == email);
            return !teacherExists;
        }

        public async Task UpdateStudentAsync(object student)
        {
            if (student is Student s)
            {
                _context.Students.Update(s);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateTeacherAsync(object teacher)
        {
            if (teacher is Teacher t)
            {
                _context.Teachers.Update(t);
                await _context.SaveChangesAsync();
            }
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
