using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Enrollments")]
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        public int GroupId { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Now;
        
        [DataType(DataType.Date)]
        public DateTime? CompletionDate { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }
        
        public bool IsCompleted { get; set; } = false;
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
        
        [ForeignKey("GroupId")]
        public virtual Group Group { get; set; } = null!;
    }
}