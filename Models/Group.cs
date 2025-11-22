using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Groups")]
    public class Group
    {
        [Key]
        public int GroupId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string GroupName { get; set; } = string.Empty;
        
        [Required]
        public int LevelId { get; set; }
        
        [Required]
        public int TeacherId { get; set; }
        
        [Required]
        public int LanguageId { get; set; } // Додано поле для мови

        [Required]
        [StringLength(100)]
        public string LevelName { get; set; } = string.Empty; // Додане поле для рівня як назви
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
        
        [Range(1, 50)]
        public int MaxStudents { get; set; } = 20;
        
        // Navigation properties
        [ForeignKey("LevelId")]
        public virtual Level Level { get; set; } = null!;
        
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;

        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; } = null!; // Language навігація
        
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Schedule> Schedules { get; set; } = new List<Schedule>();
        
        // Computed property for student count
        [NotMapped]
        public int StudentCount => Enrollments?.Count ?? 0;
    }
}