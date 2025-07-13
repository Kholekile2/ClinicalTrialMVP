using ClinicalTrial2._0.Models.DTOs;
using TinyCsvParser.Mapping;

namespace ClinicalTrial2._0.Services.CsvMapping
{
    /// <summary>
    /// CSV mapping configuration for the actual NCT CSV format
    /// Maps CSV columns to TrialUploadDto properties based on the real clinical trials CSV structure
    /// </summary>
    public class ClinicalTrialsCsvMapping : CsvMapping<TrialUploadDto>
    {
        public ClinicalTrialsCsvMapping()
        {
            // Map CSV columns to DTO properties based on the actual CSV structure:
            // 0: NCT Number, 1: Study Title, 2: Study URL, 3: Study Status, 4: Brief Summary
            // 5: Conditions, 6: Interventions, 7: Sex, 8: Age, 9: Phases
            // 10: Start Date, 11: Completion Date, 12: Locations
            
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
            MapProperty(10, x => x.TrialStartDate);     // Start Date
            MapProperty(11, x => x.TrialEndDate);       // Completion Date
            MapProperty(12, x => x.Location);           // Locations
        }
    }
}
