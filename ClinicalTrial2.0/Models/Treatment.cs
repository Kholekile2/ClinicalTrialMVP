using System.ComponentModel.DataAnnotations;

namespace ClinicalTrial2._0.Models
{
    public class Treatment
    {
        [Key]
        public int TreatmentId { get; set; }

        [Required]
        [StringLength(100)]
        public string TreatmentName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? TreatmentType { get; set; }

        // Navigation Properties
        public virtual ICollection<TrialTreatment> TrialTreatments { get; set; } = new List<TrialTreatment>();
    }
}
