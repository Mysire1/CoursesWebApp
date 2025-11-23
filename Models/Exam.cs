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
        [StringLength(50)]
        public string Level { get; set; } = string.Empty; // Статичний рівень

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExamDate { get; set; }
        
        [StringLength(255)]
        public string? Description { get; set; }
        
        // Видалено навігацію на Level
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
    }
}