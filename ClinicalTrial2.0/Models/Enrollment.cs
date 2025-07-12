using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalTrial2._0.Models
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public string ParticipantId { get; set; } = string.Empty;

        [Required]
        public int TrialId { get; set; }

        [Required]
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed, Withdrawn

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? StatusUpdatedDate { get; set; }

        // Navigation Properties
        [ForeignKey("ParticipantId")]
        public virtual ApplicationUser Participant { get; set; } = null!;

        [ForeignKey("TrialId")]
        public virtual Trial Trial { get; set; } = null!;
    }
}
