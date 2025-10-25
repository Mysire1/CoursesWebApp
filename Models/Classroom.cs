using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Classrooms")]
    public class Classroom
    {
        [Key]
        public int ClassroomId { get; set; }
        
        [Required]
        [StringLength(20)]
        public string RoomNumber { get; set; } = string.Empty;
        
        [Range(1, 100)]
        public int Capacity { get; set; } = 20;
        
        [StringLength(500)]
        public string? Equipment { get; set; }
        
        // Navigation properties
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
    }
}