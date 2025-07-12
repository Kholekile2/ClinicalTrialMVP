using System.ComponentModel.DataAnnotations;

namespace ClinicalTrial2._0.Models.ViewModels
{
    public class CreateTrialViewModel
    {
        [Required]
        [StringLength(50)]
        [Display(Name = "NCT Number")]
        public string NCTNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        [Display(Name = "Trial Name")]
        public string TrialName { get; set; } = string.Empty;

        [StringLength(500)]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Url]
        [Display(Name = "Trial URL")]
        public string? URL { get; set; }

        [Required]
        [Range(1, 4)]
        [Display(Name = "Trial Phase")]
        public int TrialPhase { get; set; }

        [StringLength(20)]
        [Display(Name = "Sex")]
        public string? Sex { get; set; }

        [StringLength(50)]
        [Display(Name = "Age Range")]
        public string? AgeRange { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Trial Start Date")]
        public DateTime TrialStartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Trial End Date")]
        public DateTime TrialEndDate { get; set; }

        [Required]
        [Display(Name = "Location")]
        public int LocationId { get; set; }

        [Required]
        [Display(Name = "Disease")]
        public int DiseaseId { get; set; }

        [Display(Name = "Treatments")]
        public List<int> TreatmentIds { get; set; } = new List<int>();

        // For dropdowns
        public IEnumerable<Location>? Locations { get; set; }
        public IEnumerable<Disease>? Diseases { get; set; }
        public IEnumerable<Treatment>? Treatments { get; set; }
    }
}
