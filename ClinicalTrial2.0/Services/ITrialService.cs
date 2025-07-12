using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.ViewModels;

namespace ClinicalTrial2._0.Services
{
    public interface ITrialService
    {
        Task<IEnumerable<Trial>> GetAllTrialsAsync();
        Task<IEnumerable<Trial>> GetActiveTrialsAsync();
        Task<Trial?> GetTrialByIdAsync(int id);
        Task<IEnumerable<Trial>> SearchTrialsAsync(string searchTerm);
        Task<IEnumerable<Trial>> GetTrialsByDiseaseAsync(int diseaseId);
        Task<IEnumerable<Trial>> GetTrialsByLocationAsync(int locationId);
        Task<IEnumerable<Trial>> GetTrialsByCreatorAsync(string creatorId);
        Task<bool> CreateTrialAsync(CreateTrialViewModel model, string creatorId);
        Task<bool> UpdateTrialAsync(Trial trial);
        Task<bool> DeleteTrialAsync(int id);
        Task<bool> EnrollParticipantAsync(string participantId, int trialId);
        Task<IEnumerable<Enrollment>> GetParticipantEnrollmentsAsync(string participantId);
        Task<IEnumerable<Enrollment>> GetTrialEnrollmentsAsync(int trialId);
        Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, string status, string? notes = null);
    }
}
