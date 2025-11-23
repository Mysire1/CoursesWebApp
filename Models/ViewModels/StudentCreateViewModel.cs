using System;
using System.ComponentModel.DataAnnotations;

namespace CoursesWebApp.Models.ViewModels
{
    public class StudentCreateViewModel
    {
        [Required(ErrorMessage = "Введіть ім'я")]
        [StringLength(100)]
        [Display(Name = "Ім'я")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть прізвище")]
        [StringLength(100)]
        [Display(Name = "Прізвище")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введіть дату народження")]
        [DataType(DataType.Date)]
        [Display(Name = "Дата народження")]
        public DateTime DateOfBirth { get; set; } = DateTime.Now.AddYears(-20);

        [StringLength(20)]
        [Display(Name = "Телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Введіть email")]
        [EmailAddress(ErrorMessage = "Некоректний email")]
        [Display(Name = "Email")]
        public string Email { get; set; } = string.Empty;

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль має бути мінімум 6 символів")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль (опційно)")]
        public string? Password { get; set; }

        [Display(Name = "Група")]
        public int? GroupId { get; set; }

        [Display(Name = "Має знижку")]
        public bool HasDiscount { get; set; } = false;

        [Range(0, 100)]
        [Display(Name = "Відсоток знижки")]
        public decimal DiscountPercentage { get; set; } = 0;

        [Display(Name = "Статус оплати")]
        public string? PaymentStatus { get; set; } = "Paid";
    }
}
