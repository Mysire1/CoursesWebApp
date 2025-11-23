using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CoursesWebApp.Models.ViewModels
{
    public class AddScheduleViewModel
    {
        [Required]
        public int GroupId { get; set; }

        [Required]
        public List<string> DaysOfWeek { get; set; } = new List<string>();

        [Required]
        public int ClassroomId { get; set; }

        [Required]
        [RegularExpression(@"^\d{2}:\d{2} ?- ?\d{2}:\d{2}$", ErrorMessage = "Введіть час у форматі 17:00 - 18:00")]
        public string TimeRange { get; set; } = "";
    }
}
