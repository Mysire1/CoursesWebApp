using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Exams")]
    public class Exam
    {
        [Key]
        public int ExamId { get; set; }
        
        [Required]
        public int LevelId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; }
        
        [StringLength(255)]
        public string? Description { get; set; }
        
        // Navigation properties
        [ForeignKey("LevelId")]
        public virtual Level Level { get; set; } = null!;
        
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}