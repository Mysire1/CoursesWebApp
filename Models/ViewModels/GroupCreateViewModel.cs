using System;
using System.ComponentModel.DataAnnotations;

namespace CoursesWebApp.Models.ViewModels
{
    public class GroupCreateViewModel
    {
        [Required]
        [Display(Name = "Назва групи")]
        public string GroupName { get; set; }

        [Required]
        [Display(Name = "Викладач")]
        public int TeacherId { get; set; }

        [Required]
        [Display(Name = "Мова")]
        public int LanguageId { get; set; }

        [Required]
        [Display(Name = "Рівень")]
        public string LevelName { get; set; }

        [Required]
        [Display(Name = "Дата старту")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
    }
}
