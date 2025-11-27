using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Schedules")]
    public class Schedule
    {
        [Key]
        public int ScheduleId { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        public int ClassroomId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string DayOfWeek { get; set; } = string.Empty;
        
        [Required]
        public TimeSpan StartTime { get; set; }
        
        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        public int TeacherId { get; set; }
        
        [Column(TypeName = "date")]
        public DateTime Date { get; set; }
        
        [StringLength(50)]
        public string Room { get; set; } = string.Empty;
        
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; } = null!;
        [ForeignKey("ClassroomId")]
        public virtual Classroom Classroom { get; set; } = null!;
    }
}
