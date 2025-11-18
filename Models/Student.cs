using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Students")]
    public class Student
    {
        [Key]
        public int StudentId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }
        
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        
        public bool HasDiscount { get; set; } = false;
        
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        // Foreign Key to Groups
        public int? GroupId { get; set; }
        
        // Navigation properties
        [ForeignKey("GroupId")]
        public virtual Group? Group { get; set; }
        
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<ExamResult> ExamResults { get; set; } = new List<ExamResult>();
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public virtual ICollection<PaymentDeferral> PaymentDeferrals { get; set; } = new List<PaymentDeferral>();
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}