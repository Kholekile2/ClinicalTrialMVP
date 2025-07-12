using ClinicalTrial2._0.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicalTrial2._0.Data.Repositories
{
    public class TrialRepository : Repository<Trial>, ITrialRepository
    {
        public TrialRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Trial>> GetTrialsWithDetailsAsync()
        {
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Include(t => t.TrialTreatments)
                    .ThenInclude(tt => tt.Treatment)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Trial?> GetTrialWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Include(t => t.TrialTreatments)
                    .ThenInclude(tt => tt.Treatment)
                .Include(t => t.Enrollments)
                    .ThenInclude(e => e.Participant)
                .FirstOrDefaultAsync(t => t.TrialId == id);
        }

        public async Task<IEnumerable<Trial>> GetTrialsByDiseaseAsync(int diseaseId)
        {
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Where(t => t.DiseaseId == diseaseId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trial>> GetTrialsByLocationAsync(int locationId)
        {
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Where(t => t.LocationId == locationId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trial>> GetTrialsByCreatorAsync(string creatorId)
        {
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.TrialTreatments)
                    .ThenInclude(tt => tt.Treatment)
                .Where(t => t.CreatedByUserId == creatorId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trial>> SearchTrialsAsync(string searchTerm)
        {
            var term = searchTerm.ToLower();
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Where(t => t.TrialName.ToLower().Contains(term) ||
                           t.Description!.ToLower().Contains(term) ||
                           t.Disease.DiseaseName.ToLower().Contains(term) ||
                           t.NCTNumber.ToLower().Contains(term))
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Trial>> GetActiveTrialsAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .Include(t => t.Location)
                .Include(t => t.Disease)
                .Include(t => t.CreatedBy)
                .Where(t => t.TrialStartDate <= today && t.TrialEndDate >= today)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }
    }
}
