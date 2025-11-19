using System.ComponentModel.DataAnnotations;

namespace CoursesWebApp.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Некоректний email")]
        [Display(Name = "Email")]
        public string Username { get; set; } = string.Empty; // Залишаємо Username для сумісності з контролером

        [Required(ErrorMessage = "Введіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Некоректний email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути мінімум 6 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; } = string.Empty;

        [DataType(DataType.Password)]
        [Display(Name = "Підтвердити пароль")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть ім'я")]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть прізвище")]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть номер телефону")]
        [Phone(ErrorMessage = "Некоректний номер телефону")]
        [StringLength(20, ErrorMessage = "Номер телефону не може бути довшим 20 символів")]
        [Display(Name = "Номер телефону")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Оберіть роль")]
        [Display(Name = "Роль")]
        public string Role { get; set; } = "Student";
    }
}