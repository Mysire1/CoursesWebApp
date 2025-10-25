using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("TeacherLanguages")]
    public class TeacherLanguage
    {
        [Key]
        public int TeacherLanguageId { get; set; }
        
        [Required]
        public int TeacherId { get; set; }
        
        [Required]
        public int LanguageId { get; set; }
        
        // Navigation properties
        [ForeignKey("TeacherId")]
        public virtual Teacher Teacher { get; set; } = null!;
        
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; } = null!;
    }
}