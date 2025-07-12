using ClinicalTrial2._0.Models;

namespace ClinicalTrial2._0.Services
{
    public interface IReferenceDataService
    {
        Task<IEnumerable<Location>> GetAllLocationsAsync();
        Task<IEnumerable<Disease>> GetAllDiseasesAsync();
        Task<IEnumerable<Treatment>> GetAllTreatmentsAsync();
        Task<Location?> GetLocationByIdAsync(int id);
        Task<Disease?> GetDiseaseByIdAsync(int id);
        Task<Treatment?> GetTreatmentByIdAsync(int id);
    }
}
