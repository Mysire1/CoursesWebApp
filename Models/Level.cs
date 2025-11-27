using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CoursesWebApp.Models
{
    [Table("Levels")]
    public class Level
    {
        [Key]
        public int LevelId { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string? Description { get; set; }
        
        [Required]
        public int LanguageId { get; set; }
        
        [Required]
        [Range(0, double.MaxValue)]
        public decimal BaseCost { get; set; }
        
        [Range(1, 24)]
        public int DurationMonths { get; set; } = 3;
        
        [ForeignKey("LanguageId")]
        public virtual Language Language { get; set; } = null!;
        
        public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
    }
}