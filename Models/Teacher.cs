using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Teachers")]
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string LastName { get; set; } = string.Empty;
        
        [StringLength(255)]
        [EmailAddress]
        public string? Email { get; set; }
        
        [StringLength(20)]
        public string? Phone { get; set; }
        
        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }
        
        // Navigation properties
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
        public virtual ICollection<TeacherLanguage> TeacherLanguages { get; set; } = new List<TeacherLanguage>();
        
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";
    }
}