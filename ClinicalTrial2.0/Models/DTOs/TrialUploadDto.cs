namespace ClinicalTrial2._0.Models.DTOs
{
    /// <summary>
    /// Data Transfer Object for CSV trial upload
    /// Represents the structure of trial data coming from CSV files
    /// </summary>
    public class TrialUploadDto
    {
        public string TrialName { get; set; } = string.Empty;
        public string NCTNumber { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public string TrialPhase { get; set; } = string.Empty;
        public string Sex { get; set; } = string.Empty;
        public string AgeRange { get; set; } = string.Empty;
        public DateTime TrialStartDate { get; set; }
        public DateTime TrialEndDate { get; set; }
        public string Location { get; set; } = string.Empty; // Comma-separated location data
        public string Condition { get; set; } = string.Empty; // Disease/condition
        public string Description { get; set; } = string.Empty; // Trial description
        public string Status { get; set; } = string.Empty;
        public string Treatment { get; set; } = string.Empty; // Pipe-separated treatments
    }
}
