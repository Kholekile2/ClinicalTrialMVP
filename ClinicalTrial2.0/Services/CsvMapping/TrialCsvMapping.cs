using ClinicalTrial2._0.Models.DTOs;
using TinyCsvParser.Mapping;

namespace ClinicalTrial2._0.Services.CsvMapping
{
    /// <summary>
    /// CSV mapping configuration for TinyCsvParser
    /// Maps CSV columns to TrialUploadDto properties based on the CSV structure
    /// defined in the analysis document
    /// </summary>
    public class TrialCsvMapping : CsvMapping<TrialUploadDto>
    {
        public TrialCsvMapping()
        {
            // Map CSV columns to DTO properties based on the documented structure:
            // 0: NCTNumber, 1: TrialName, 2: URL, 3: Status, 4: TrialDescription
            // 5: Condition, 6: Treatment, 7: Sex, 8: Age, 9: TrialPhase
            // 10: TrialStartDate, 11: TrialEndDate, 12: Location
            
            MapProperty(0, x => x.NCTNumber);
            MapProperty(1, x => x.TrialName);
            MapProperty(2, x => x.URL);
            MapProperty(3, x => x.Status);
            MapProperty(4, x => x.Description);
            MapProperty(5, x => x.Condition);
            MapProperty(6, x => x.Treatment);
            MapProperty(7, x => x.Sex);
            MapProperty(8, x => x.AgeRange);
            MapProperty(9, x => x.TrialPhase);
            MapProperty(10, x => x.TrialStartDate);
            MapProperty(11, x => x.TrialEndDate);
            MapProperty(12, x => x.Location);
        }
    }
}
