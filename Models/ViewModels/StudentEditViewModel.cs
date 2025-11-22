using System.ComponentModel.DataAnnotations;

namespace CoursesWebApp.Models.ViewModels
{
    public class StudentEditViewModel
    {
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
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public int? GroupId { get; set; }
        public bool HasDiscount { get; set; } = false;
        [Range(0, 100)]
        public decimal DiscountPercentage { get; set; } = 0;
        
        [StringLength(50)]
        public string? Status { get; set; }
    }
}