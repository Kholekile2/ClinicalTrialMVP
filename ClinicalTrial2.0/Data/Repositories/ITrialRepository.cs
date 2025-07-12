using ClinicalTrial2._0.Models;

namespace ClinicalTrial2._0.Data.Repositories
{
    public interface ITrialRepository : IRepository<Trial>
    {
        Task<IEnumerable<Trial>> GetTrialsWithDetailsAsync();
        Task<Trial?> GetTrialWithDetailsAsync(int id);
        Task<IEnumerable<Trial>> GetTrialsByDiseaseAsync(int diseaseId);
        Task<IEnumerable<Trial>> GetTrialsByLocationAsync(int locationId);
        Task<IEnumerable<Trial>> GetTrialsByCreatorAsync(string creatorId);
        Task<IEnumerable<Trial>> SearchTrialsAsync(string searchTerm);
        Task<IEnumerable<Trial>> GetActiveTrialsAsync();
    }
}
