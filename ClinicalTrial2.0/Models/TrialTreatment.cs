using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalTrial2._0.Models
{
    public class TrialTreatment
    {
        [Key]
        public int TrialTreatmentId { get; set; }

        [Required]
        public int TrialId { get; set; }

        [Required]
        public int TreatmentId { get; set; }

        // Navigation Properties
        [ForeignKey("TrialId")]
        public virtual Trial Trial { get; set; } = null!;

        [ForeignKey("TreatmentId")]
        public virtual Treatment Treatment { get; set; } = null!;
    }
}
