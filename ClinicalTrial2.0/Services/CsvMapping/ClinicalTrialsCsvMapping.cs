using ClinicalTrial2._0.Models.DTOs;
using TinyCsvParser.Mapping;

namespace ClinicalTrial2._0.Services.CsvMapping
{
    /// <summary>
    /// CSV mapping configuration for the actual NCT CSV format
    /// Maps CSV columns to TrialUploadDto properties based on the real clinical trials CSV structure
    /// Handles flexible data type conversion and missing columns
    /// </summary>
    public class ClinicalTrialsCsvMapping : CsvMapping<TrialUploadDto>
    {
        public ClinicalTrialsCsvMapping()
        {
            // Map CSV columns to DTO properties with safe column access
            // Uses MapProperty with error handling for missing columns
            
            MapProperty(0, x => x.NCTNumber);           // NCT Number
            MapProperty(1, x => x.TrialName);           // Study Title  
            MapProperty(2, x => x.URL);                 // Study URL
            MapProperty(3, x => x.Status);              // Study Status
            MapProperty(4, x => x.Description);         // Brief Summary
            MapProperty(5, x => x.Condition);           // Conditions
            MapProperty(6, x => x.Treatment);           // Interventions
            MapProperty(7, x => x.Sex);                 // Sex
            MapProperty(8, x => x.AgeRange);            // Age
            MapProperty(9, x => x.TrialPhase);          // Phases
            MapProperty(10, x => x.TrialStartDate);     // Start Date (as string for flexible parsing)
            MapProperty(11, x => x.TrialEndDate);       // Completion Date (as string for flexible parsing)
            MapProperty(12, x => x.Location);           // Locations
        }
    }
}
