using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Payments")]
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        // Navigation properties
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
    }
}