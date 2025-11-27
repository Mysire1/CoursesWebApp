using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("PaymentDeferrals")]
    public class PaymentDeferral
    {
        [Key]
        public int PaymentDeferralId { get; set; }
        
        [Required]
        public int StudentId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal DeferredAmount { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime DeferralDate { get; set; } = DateTime.Now;
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }
        
        [StringLength(500)]
        public string? Reason { get; set; }
        
        public bool IsPaid { get; set; } = false;
        
        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;
    }
}