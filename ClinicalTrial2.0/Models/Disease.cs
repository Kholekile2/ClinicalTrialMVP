using System.ComponentModel.DataAnnotations;

namespace ClinicalTrial2._0.Models
{
    public class Disease
    {
        [Key]
        public int DiseaseId { get; set; }

        [Required]
        [StringLength(100)]
        public string DiseaseName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        // Navigation Properties
        public virtual ICollection<Trial> Trials { get; set; } = new List<Trial>();
    }
}
