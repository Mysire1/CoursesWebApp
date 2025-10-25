using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("ExamResults")]
    public class ExamResult
    {
        [Key]
        public int ExamResultId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int ExamId { get; set; }
        
        [Required]
        [Range(0, 100)]
        public int Grade { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; }
        
        public bool IsPassed => Grade >= 60;
        
        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        
        [ForeignKey("ExamId")]
        public virtual Exam Exam { get; set; } = null!;
    }
}