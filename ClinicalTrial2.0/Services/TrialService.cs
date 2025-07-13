using ClinicalTrial2._0.Data;
using ClinicalTrial2._0.Data.Repositories;
using ClinicalTrial2._0.Models;
using ClinicalTrial2._0.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial2._0.Services
{
    public class TrialService : ITrialService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public TrialService(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public async Task<IEnumerable<Trial>> GetAllTrialsAsync()
        {
            return await _unitOfWork.Trials.GetTrialsWithDetailsAsync();
        }

        public async Task<IEnumerable<Trial>> GetActiveTrialsAsync()
        {
            return await _unitOfWork.Trials.GetActiveTrialsAsync();
        }

        public async Task<Trial?> GetTrialByIdAsync(int id)
        {
            return await _unitOfWork.Trials.GetTrialWithDetailsAsync(id);
        }

        public async Task<IEnumerable<Trial>> SearchTrialsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllTrialsAsync();

            return await _unitOfWork.Trials.SearchTrialsAsync(searchTerm);
        }

        public async Task<IEnumerable<Trial>> GetTrialsByDiseaseAsync(int diseaseId)
        {
            return await _unitOfWork.Trials.GetTrialsByDiseaseAsync(diseaseId);
        }

        public async Task<IEnumerable<Trial>> GetTrialsByLocationAsync(int locationId)
        {
            return await _unitOfWork.Trials.GetTrialsByLocationAsync(locationId);
        }

        public async Task<IEnumerable<Trial>> GetTrialsByCreatorAsync(string creatorId)
        {
            return await _context.Trials
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.Enrollments)
                .Where(t => t.CreatedByUserId == creatorId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<bool> CreateTrialAsync(CreateTrialViewModel model, string creatorId)
        {
            try
            {
                // Check if NCT Number already exists
                var existingTrial = await _unitOfWork.Trials.FirstOrDefaultAsync(t => t.NCTNumber == model.NCTNumber);
                if (existingTrial != null)
                    return false;

                var trial = new Trial
                {
                    NCTNumber = model.NCTNumber,
                    TrialName = model.TrialName,
                    Description = model.Description,
                    URL = model.URL,
                    TrialPhase = model.TrialPhase,
                    Sex = model.Sex,
                    AgeRange = model.AgeRange,
                    TrialStartDate = model.TrialStartDate,
                    TrialEndDate = model.TrialEndDate,
                    LocationId = model.LocationId,
                    DiseaseId = model.DiseaseId,
                    CreatedByUserId = creatorId,
                    CreatedDate = DateTime.UtcNow
                };

                await _unitOfWork.Trials.AddAsync(trial);
                await _unitOfWork.SaveChangesAsync();

                // Add treatments if any
                if (model.TreatmentIds?.Any() == true)
                {
                    var trialTreatments = model.TreatmentIds.Select(treatmentId => new TrialTreatment
                    {
                        TrialId = trial.TrialId,
                        TreatmentId = treatmentId
                    });

                    await _unitOfWork.TrialTreatments.AddRangeAsync(trialTreatments);
                    await _unitOfWork.SaveChangesAsync();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateTrialAsync(Trial trial)
        {
            try
            {
                _unitOfWork.Trials.Update(trial);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteTrialAsync(int id)
        {
            try
            {
                var trial = await _unitOfWork.Trials.GetByIdAsync(id);
                if (trial == null)
                    return false;

                _unitOfWork.Trials.Remove(trial);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> EnrollParticipantAsync(string participantId, int trialId)
        {
            try
            {
                // Check if already enrolled
                var existingEnrollment = await _unitOfWork.Enrollments
                    .FirstOrDefaultAsync(e => e.ParticipantId == participantId && e.TrialId == trialId);

                if (existingEnrollment != null)
                    return false;

                var enrollment = new Enrollment
                {
                    ParticipantId = participantId,
                    TrialId = trialId,
                    EnrollmentDate = DateTime.UtcNow,
                    Status = "Pending"
                };

                await _unitOfWork.Enrollments.AddAsync(enrollment);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<IEnumerable<Enrollment>> GetParticipantEnrollmentsAsync(string participantId)
        {
            return await _context.Enrollments
                .Include(e => e.Trial)
                    .ThenInclude(t => t.Location)
                .Include(e => e.Trial)
                    .ThenInclude(t => t.Disease)
                .Where(e => e.ParticipantId == participantId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Enrollment>> GetTrialEnrollmentsAsync(int trialId)
        {
            var trial = await _unitOfWork.Trials.GetByIdAsync(trialId);
            if (trial == null)
                throw new ArgumentException("Trial not found", nameof(trialId));

            return await _context.Enrollments
                .Include(e => e.Trial)
                    .ThenInclude(t => t.Location)
                .Include(e => e.Trial)
                    .ThenInclude(t => t.Disease)
                .Where(e => e.TrialId == trialId)
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();
        }

        public async Task<bool> UpdateEnrollmentStatusAsync(int enrollmentId, string status, string? notes = null)
        {
            try
            {
                var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(enrollmentId);
                if (enrollment == null)
                    return false;

                enrollment.Status = status;
                enrollment.StatusUpdatedDate = DateTime.UtcNow;
                
                if (!string.IsNullOrEmpty(notes))
                {
                    enrollment.Notes = notes;
                }

                _unitOfWork.Enrollments.Update(enrollment);
                await _unitOfWork.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
