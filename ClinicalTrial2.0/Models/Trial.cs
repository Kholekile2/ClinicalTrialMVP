using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalTrial2._0.Models
{
    public class Trial
    {
        [Key]
        public int TrialId { get; set; }

        [Required]
        [StringLength(50)]
        public string NCTNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string TrialName { get; set; } = string.Empty;

        [StringLength(2000)]  // Increased from 500 to 2000 characters
        public string? Description { get; set; }

        [Url]
        public string? URL { get; set; }

        [Range(1, 4)]
        public int TrialPhase { get; set; }

        [StringLength(20)]
        public string? Sex { get; set; }

        [StringLength(50)]
        public string? AgeRange { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TrialStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TrialEndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Foreign Keys
        [Required]
        public string CreatedByUserId { get; set; } = string.Empty;

        [Required]
        public int LocationId { get; set; }

        [Required]
        public int DiseaseId { get; set; }

        // Navigation Properties
        [ForeignKey("CreatedByUserId")]
        public virtual ApplicationUser CreatedBy { get; set; } = null!;

        [ForeignKey("LocationId")]
        public virtual Location Location { get; set; } = null!;

        [ForeignKey("DiseaseId")]
        public virtual Disease Disease { get; set; } = null!;

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<TrialTreatment> TrialTreatments { get; set; } = new List<TrialTreatment>();
        public virtual ICollection<TrialTranslation> TrialTranslations { get; set; } = new List<TrialTranslation>();
    }
}
