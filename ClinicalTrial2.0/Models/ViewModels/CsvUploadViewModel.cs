using System.ComponentModel.DataAnnotations;

namespace ClinicalTrial2._0.Models.ViewModels
{
    /// <summary>
    /// View model for CSV trial upload page
    /// </summary>
    public class CsvUploadViewModel
    {
        [Required(ErrorMessage = "Please select a CSV file")]
        [Display(Name = "CSV File")]
        public IFormFile? CsvFile { get; set; }

        [Display(Name = "Upload Results")]
        public CsvUploadResultViewModel? UploadResult { get; set; }
    }

    /// <summary>
    /// View model for displaying CSV upload results
    /// </summary>
    public class CsvUploadResultViewModel
    {
        public bool Success { get; set; }
        public int TrialsProcessed { get; set; }
        public int TrialsImported { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
        public string Message { get; set; } = string.Empty;
        public string? SuccessMessage => Success ? Message : null;
        public string? ErrorMessage => !Success ? Message : null;
        public bool HasErrors => Errors.Any();
    }
}
