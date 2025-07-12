using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicalTrial2._0.Models
{
    public class TrialTranslation
    {
        [Key]
        public int TranslationId { get; set; }

        [Required]
        public int TrialId { get; set; }

        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty; // e.g., "xh", "zu", "af"

        [Required]
        [StringLength(200)]
        public string TranslatedTrialName { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? TranslatedDescription { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        [ForeignKey("TrialId")]
        public virtual Trial Trial { get; set; } = null!;
    }
}
