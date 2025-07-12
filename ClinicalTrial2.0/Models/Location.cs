using System.ComponentModel.DataAnnotations;

namespace ClinicalTrial2._0.Models
{
    public class Location
    {
        [Key]
        public int LocationId { get; set; }

        [Required]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Province { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Country { get; set; } = string.Empty;

        [StringLength(10)]
        public string? PostalCode { get; set; }

        [StringLength(200)]
        public string? Address { get; set; }

        // Navigation Properties
        public virtual ICollection<Trial> Trials { get; set; } = new List<Trial>();

        public string FullAddress => $"{Address}, {City}, {Province}, {Country}";
    }
}
